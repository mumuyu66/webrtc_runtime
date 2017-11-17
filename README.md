# webrtc_runtime
webrtc_runtime

1.   android/  安卓的编译环境，编译arm时记得在jni/Android.mk去掉下面几行
      ../../webrtc/common_audio/fir_filter_sse.cc \
                                        
      ../../webrtc/common_audio/resampler/sinc_resampler_sse.cc \
                                        
      ../../webrtc/modules/audio_processing/utility/ooura_fft_sse2.cc \
                                        
      ../../webrtc/modules/audio_processing/aec/aec_core_sse2.cc \  

 2. audio/   测试用的音源

 3. build/webrtc_mac/  mac编译环境

 4. webrtc/   webrtc 源码，从https://chromium.googlesource.com/external/webrtc/+/branch-heads/61 抽出
 
 5. WebRTCDemo/   unity3d 测试Demo 

 6. CMakeLists.txt  win64 makelist
 
 
 -----------------------------------------------------------------------------------
 WebRTCDemo 运行时要注意的地方
 
 WebRTCDemo 中的AudioDefine.cs里有一系列的设置
 
 其中 DEBUG_NO 默认为 3，这种模式下，只消除网络话音（不能消除背景音乐）
 
 DEBUG_NO = 4 时，要将AudioManager的SampleRate设置为48000，并上手机上测试
 
 
 
 MS_IN_SEND_CARDBUF 是消除效果的关键

 MS_IN_SEND_CARDBUF = delay
 
它的算法是 
 delay = (t_render - t_analyze) + (t_process - t_capture)
where:
   - t_analyze is the time a frame is passed to AnalyzeReverseStream() and
     t_render is the time the first sample of the same frame is rendered by
     the audio hardware.
   - t_capture is the time the first sample of a frame is captured by the
     audio hardware and t_pull is the time the same frame is passed to
     ProcessStream().
	 
Sets the |delay| in ms between AnalyzeReverseStream() receiving a far-end frame and ProcessStream() receiving a near-end frame containing the corresponding echo. 
On the client-side this can be expressed as delay = (t_render - t_analyze) + (t_process - t_capture)

where,

t_analyze is the time a frame is passed to AnalyzeReverseStream() and t_render is the time the first sample of the same frame is rendered by the audio hardware.
t_capture is the time the first sample of a frame is captured by the audio hardware and t_pull is the time the same frame is passed to
ProcessStream().
 
 
安卓下 MS_IN_SEND_CARDBUF = 95; 这个值在不同安卓机型上要自己微调

IOS下  MS_IN_SEND_CARDBUF = 65; 
 
 -------------------------------------------------------------------------------------

ios10设备，没麦克风权限的解决方案

1.在项目中找到info.plist
文件，右键点击以 Source Code形式打开

2.添加以下键值对
Privacy - Microphone Usage Description 是否允许此App使用你的麦克风？