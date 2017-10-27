#include "webrtc/modules/audio_processing/include/audio_processing.h"

using namespace std;
using namespace webrtc;

int main()
{
    AudioProcessing* apm = AudioProcessing::Create();
    // Repeate render and capture processing for the duration of the call...
    // Start a new call...
    apm->Initialize();
    
    // Close the application...
    delete apm;
} 
