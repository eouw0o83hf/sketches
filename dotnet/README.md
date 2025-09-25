This is the C# sketching renderer.

## Dependencies

### `ffmpeg`
Install `ffmpeg`, [here's the documentation for it](https://github.com/rosenbjerg/FFMpegCore?tab=readme-ov-file#installation). Relevant bits copied as of Sept 2025:

**Windows (using choco)**
command: choco install ffmpeg -y
location: C:\ProgramData\chocolatey\lib\ffmpeg\tools\ffmpeg\bin

**Mac OSX**
command: brew install ffmpeg mono-libgdiplus
location: /usr/local/bin

**Ubuntu**
command: sudo apt-get install -y ffmpeg libgdiplus
location: /usr/bin
