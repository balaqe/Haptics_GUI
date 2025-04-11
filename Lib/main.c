#include <stdio.h>
#include <stdlib.h>
#include <math.h>
#include "portaudio.h"

#define SAMPLE_RATE         (44100)
#define FRAMES_PER_BUFFER   (1)
#define DEBUG_CH (1)

#ifndef M_PI
#define M_PI  (3.14159265)
#endif

#define TABLE_SIZE   (200)


/*******************************************************************/
int play(float *data_in, int len, int num_ch) { // waveform data, length of waveforms, number of channels
    float waveforms[len][num_ch];
    PaStreamParameters outputParameters;
    PaStream *stream;
    PaError err;
    float buffer[FRAMES_PER_BUFFER][num_ch]; // Specify number of channels
    float debug_buff[len];
    int i, j, k;
    int bufferCount;
    int sample_count;

    // DEBUG
    FILE *wf_file;
    wf_file = fopen("./Debug/input_wave_log.csv", "w+");
    if (wf_file == NULL) printf("Could not open waveform_log.csv");
    FILE *og_file;
    og_file = fopen("./Debug/played_wave_log.csv", "w+");
    if (wf_file == NULL) printf("Could not open original_sine_log.csv");
    // printf("len: %d    num_ch: %d\n", len, num_ch);
    // ** DEBUG

    sample_count = 0;
    for (i=0; i<num_ch; i++) {
        for (j=0; j<len; j++) {
            waveforms[j][i] = data_in[sample_count];
            sample_count++;
        }
    }

    err = Pa_Initialize();
    if( err != paNoError ) goto error;

    outputParameters.device = Pa_GetDefaultOutputDevice(); /* default output device */
    if (outputParameters.device == paNoDevice) {
      fprintf(stderr,"Error: No default output device.\n");
      goto error;
    }
    outputParameters.channelCount = num_ch;       // specify number of channels
    outputParameters.sampleFormat = paFloat32; /* 32 bit floating point output */
    outputParameters.suggestedLatency = 0.050; // Pa_GetDeviceInfo( outputParameters.device )->defaultLowOutputLatency;
    outputParameters.hostApiSpecificStreamInfo = NULL;

    err = Pa_OpenStream(
              &stream,
              NULL, /* no input */
              &outputParameters,
              SAMPLE_RATE,
              FRAMES_PER_BUFFER,
              paClipOff,      /* we won't output out of range samples so don't bother clipping them */
              NULL, /* no callback, use blocking API */
              NULL ); /* no callback, so no callback userData */
    if( err != paNoError ) goto error;


    err = Pa_StartStream( stream );
    if( err != paNoError ) goto error;

    sample_count = 0;
    bufferCount = len / FRAMES_PER_BUFFER;

    for( i=0; i < bufferCount; i++ ) {
        for( j=0; j < FRAMES_PER_BUFFER; j++ ) {
            for ( k=0; k<num_ch; k++ ) {
                buffer[j][k] = waveforms[sample_count][k];
            }
            
            // DEBUG
            debug_buff[sample_count] = buffer[j][DEBUG_CH];
            // ** DEBUG

            sample_count++;
        }

        err = Pa_WriteStream( stream, buffer, FRAMES_PER_BUFFER );
        if( err != paNoError ) goto error;
    }  
    Pa_Sleep(100);

    // DEBUG
    for (j=0; j<len; j++) {
        fprintf(wf_file, "%d,%.2f\n", j, waveforms[j][DEBUG_CH]);
    }
    for (j=0; j<sample_count; j++) {
        fprintf(og_file, "%d,%.2f\n", j, debug_buff[j]);
    }
    fclose(wf_file);
    fclose(og_file);
    // ** DEBUG

    err = Pa_StopStream( stream );
    if( err != paNoError ) goto error;


    err = Pa_CloseStream( stream );
    if( err != paNoError ) goto error;

    Pa_Terminate();
    
    return err;

error:
    fprintf( stderr, "An error occured while using the portaudio stream\n" );
    fprintf( stderr, "Error number: %d\n", err );
    fprintf( stderr, "Error message: %s\n", Pa_GetErrorText( err ) );
	if( err == paUnanticipatedHostError ) {
		const PaHostErrorInfo *hostErrorInfo = Pa_GetLastHostErrorInfo();
		fprintf( stderr, "Host API error = #%ld, hostApiType = %d\n", hostErrorInfo->errorCode, hostErrorInfo->hostApiType );
		fprintf( stderr, "Host API error = %s\n", hostErrorInfo->errorText );
	}
    Pa_Terminate();

    // DEBUG
    fclose(wf_file);
    fclose(og_file);
    // ** DEBUG

    return err; 
}