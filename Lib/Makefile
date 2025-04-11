PA_INC = /opt/homebrew/Cellar/portaudio/19.7.0/include
PA_LIB = /opt/homebrew/Cellar/portaudio/19.7.0/lib

default: all

all: livePlotter.dll delO

# Compile main.c with the include flag for PortAudio headers and PIC flag if building a shared library.
main.o: main.c
	gcc -O -fPIC -c main.c -I$(PA_INC)

# Link to create a DLL.
livePlotter.dll: main.o libportaudio.a
	# -shared creates a DLL, and -lportaudio links the PortAudio library.
	gcc -shared -o soundPlayer.dll main.o -L$(PA_LIB) -lportaudio -lm

# Optionally, you might have a target to build libportaudio.a if you are compiling PortAudio yourself.
# If PortAudio is installed in your system this target may not be necessary.
libportaudio.a:
	@echo "Assuming libportaudio.a is already available in $(PA_LIB)."
	@# If you needed to create it manually, you might run:
	@# ar rcs libportaudio.a <list-of-object-files>

delO:
	rm *.o

clean:
	rm -f soundPlayer.dll *.o *.a
