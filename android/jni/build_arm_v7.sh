echo APP_ABI := armeabi-v7a> ./Application.mk
echo APP_STL := c++_static >> ./Application.mk
NDK=/Users/yangyiqiang/work/git/android-ndk-r12b
NFK=$NDK/toolchains/arm-linux-androideabi-4.9/prebuilt/darwin-x86_64/bin/arm-linux-androideabi-

make HOST_CC="g++ -m32 -ffast-math -O3" \
CROSS=$NFK \
TARGET=android-21
NDKLEVEL=21
TARGET_SYS=Linux \
TARGET_FLAGS="--sysroot $NDK/platforms/android-21/arch-arm -march=armv7-a -Wl,--fix-cortex-a8"

echo "armv7 ndk-build"
ndk-build -j8
mv ../libs/armeabi-v7a/libwebrtc.so ../Plugins/armeabi-v7a/