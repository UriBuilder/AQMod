﻿namespace Aequu2.Core.Audio;

public interface IAudioEffect {
    byte[] ModifyBuffer(byte[] buffer, int sampleRate, int channels);
}
