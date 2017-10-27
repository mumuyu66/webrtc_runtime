cd android/jni
ndk-build clean
./build_arm_v7.sh
echo "build arm_v7 end"
./build_x86.sh
echo "build x86 end"
pwd
cd ../../

