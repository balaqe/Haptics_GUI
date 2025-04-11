# Guide for compiling with PortAudio

## Install PortAudio

Download from official site: https://files.portaudio.com/download.html

1. Unpack into desired folder
```
tar -xvf filename.tar
```

2. Go into PortAudio folder
```
cd portaudio
```

3. Run installation
```
./configure && make
```

4. Install Make (may require sudo permissions)
```
make install
```

If it requires sudo permissions, then
```
sudo make install
```

You can verify your successful Make installation with
```
make --version
```

5. Verify path of installation
Look for the /include and /lib folders under portaudio.

On macOS, common install path is either
```
/opt/homebrew/Cellar/portaudio/<version>
```
or
```
/usr/local
```

If neither folder has the folders, search for the name of the libportaudio.a and portaudio.h files on your system

6. Update the /include and /lib paths in the Makefile if needed