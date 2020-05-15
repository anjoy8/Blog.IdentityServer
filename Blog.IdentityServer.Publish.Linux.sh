git pull;
rm -rf .PublishFiles;
dotnet build;
dotnet publish -o /home/Blog.IdentityServer/Blog.IdentityServer/bin/Debug/netcoreapp3.1;
cp -r /home/Blog.IdentityServer/Blog.IdentityServer/bin/Debug/netcoreapp3.1 .PublishFiles;
echo "Successfully!!!! ^ please see the file .PublishFiles";