# 停止容器
docker stop idscontainer
# 删除容器
docker rm idscontainer
# 删除镜像
docker rmi laozhangisphi/idsimg
# 切换目录
cd /home/Blog.IdentityServer
# 发布项目
./Blog.IdentityServer.Publish.Linux.sh
# 进入目录
cd /home/Blog.IdentityServer/.PublishFiles/
# 编译镜像
docker build -t laozhangisphi/idsimg .
# 生成容器
docker run --name=idscontainer -v /etc/localtime:/etc/localtime -it -p 5004:5004 laozhangisphi/idsimg
# 启动容器
docker start idscontainer
