# The-Unspoken
Contest Entry
EchoVision是什么
EchoVision是一个混合现实艺术体验项目，它模拟蝙蝠的回声定位系统，将用户的声音在周围环境中进行视觉化呈现。这使用户能够体验作为蝙蝠的感觉。
EchoVision是开源的，作为HoloKit的示例项目，HoloKit是由Holo Interactive开发的开源混合现实头显。
EchoVision专为iOS和visionOS设计。iOS版本兼容配备LiDAR摄像头的iPhone，而visionOS版本专门为Apple Vision Pro设计。
如何玩
iOS版本
EchoVision需要配备LiDAR摄像头的iPhone型号，因为EchoVision使用了Apple ARKit的网格化和人体分割功能。
你可以通过在App Store搜索"EchoVision"来下载应用。
单声道模式
在此模式下，用户无需HoloKit即可与EchoVision互动。
立体声模式
在此模式下，用户可以通过HoloKit获得更沉浸式的AR体验。
visionOS版本
通过在App Store搜索"EchoVision Pro"下载应用。
启动应用后，应用将在完全沉浸模式下运行，默认情况下周围环境会完全变黑。
发出声音来查看效果。
如何编译？
系统要求
macOS 15+，需要搭载Apple Silicon芯片的电脑
Unity 6.1+
iOS版本
在构建配置中将构建配置文件改为"iOS"
visionOS版本
在构建配置中将构建配置文件改为"visionOS"
已知问题
由于音频分析器和录音器同时占用，录音时麦克风可能无法正常工作。
关于从iOS版本迁移的重要说明
我们使用ARFoundation的ARMeshManager来生成周围环境的网格。根据iOS的实际实践和各种讨论，ARMeshManager生成的网格应该保持在(0,0,0)位置，而网格中的顶点将位于实际坐标。
然而，在VisionPro中使用ARMeshManager时，ARMeshManager生成的所有网格都具有相同的非零位置和旋转，这导致直接使用网格中顶点的坐标来生成效果时出现问题。
请记住首先将局部坐标转换为世界坐标。这个问题在本项目中已经得到处理。
