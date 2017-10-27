echo APP_ABI := x86 > ./Application.mk
echo APP_STL := c++_static >> ./Application.mk
NDK=/Users/yangyiqiang/work/git/android-ndk-r12b
NFK=$NDK/toolchains/x86-4.9/prebuilt/darwin-x86_64/bin/i686-linux-android-

make HOST_CC="g++ -m32 -ffast-math -O3" \
CROSS=$NFK \
TARGET_SYS=Linux \
TARGET_FLAGS="--sysroot $NDK/platforms/android-21/arch-x86 -Wl,--fix-cortex-a8"

ndk-build -j8
mv ../libs/x86/libwebrtc.so ../Plugins/x86/
