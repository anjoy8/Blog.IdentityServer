
![Logo](https://github.com/anjoy8/Blog.IdentityServer/blob/master/Blog.IdentityServer/wwwroot/logofull.png)

&nbsp;
&nbsp;

## 给个星星! ⭐️
如果你喜欢这个项目或者它帮助你, 请给 Star~（辛苦星咯）

*********************************************************

  
  <ul>
<li></li>
<li><a id="post_title_link_10529982" href="https://www.bilibili.com/video/BV1vC4y1p7Za?p=14">前端Blog.Admin-后端Blog.Core-认证中心(本项目) 快速启动 </a></li>
<li></li>
</ul>

## Tips：
```
 /*
  * 本项目同时支持Mysql和Sqlserver，我一直使用的是Mysql，所以Mysql的迁移文件已经配置好，在Data文件夹下，
  * 直接执行update-database xxxx,那三步即可。如果你使用sqlserver，可以先从迁移开始，下边有步骤
  * 
  * 当然你也可以把Data文件夹除了ApplicationDbContext.cs文件外都删掉，自己重新做迁移。
  * 迁移完成后，执行dotnet run /seed
  *  1、PM> add-migration InitialIdentityServerPersistedGrantDbMigrationMysql -c PersistedGrantDbContext -o Data/MigrationsMySql/IdentityServer/PersistedGrantDb 
     Build started...
     Build succeeded.
     To undo this action, use Remove-Migration.
     2、PM> update-database -c PersistedGrantDbContext
     Build started...
     Build succeeded.
     Applying migration '20200509165052_InitialIdentityServerPersistedGrantDbMigrationMysql'.
     Done.
     3、PM> add-migration InitialIdentityServerConfigurationDbMigrationMysql -c ConfigurationDbContext -o Data/MigrationsMySql/IdentityServer/ConfigurationDb
     Build started...
     Build succeeded.
     To undo this action, use Remove-Migration.
     4、PM> update-database -c ConfigurationDbContext
     Build started...
     Build succeeded.
     Applying migration '20200509165153_InitialIdentityServerConfigurationDbMigrationMysql'.
     Done.
     5、PM> add-migration AppDbMigration -c ApplicationDbContext -o Data/MigrationsMySql
     Build started...
     Build succeeded.
     To undo this action, use Remove-Migration.
     6、PM> update-database -c ApplicationDbContext
     Build started...
     Build succeeded.
     Applying migration '20200509165505_AppDbMigration'.
     Done.
  * 
  */



```


*****************************************************
### 跟踪教程


博客园：https://www.cnblogs.com/laozhang-is-phi/  
视频：https://www.bilibili.com/video/av76828468  

<ul>
<li><a id="post_title_link_10483922" href="https://www.cnblogs.com/laozhang-is-phi/p/10483922.html">01 ║ 授权服务器 IdentityServer4 开篇讲&amp;计划书</a></li>
<li><a id="post_title_link_10529982" href="https://www.cnblogs.com/laozhang-is-phi/p/10529982.html">02 ║ 基础知识集合 &amp; 项目搭建一</a></li>
<li><a id="post_title_link_10660403" href="https://www.cnblogs.com/laozhang-is-phi/p/10660403.html">03 ║ 详解授权持久化 &amp; 用户数据迁移</a></li>
<li><a id="post_title_link_10911438" href="https://www.cnblogs.com/laozhang-is-phi/p/10911438.html">04 ║ 用户数据管理 &amp; 前后端授权联调</a></li>
<li><a class="entry" href="https://www.cnblogs.com/laozhang-is-phi/p/11844395.html" target="_blank">05 ║ 多项目集成统一认证中心的思考</a></li>
<li><a class="entry" href="https://www.cnblogs.com/laozhang-is-phi/p/rolemanager-one.html" target="_blank">06 ║ 统一角色管理（上）</a>&nbsp;</li>
<li><a class="entry" href="https://www.cnblogs.com/laozhang-is-phi/p/vue-core-ids.html" target="_blank">07 ║ 客户端、服务端、授权中心全线打通</a>&nbsp;</li>
<li></li>
</ul>



微信公众号：https://mvp.neters.club/MVP_ids4_2020/2020  

<ul>
<li><a id="post_title_link_10483922" href="http://apk.neters.club/api/Blog/GoUrl?id=133">【Ids4实战】分模块保护资源API</a></li>
<li><a id="post_title_link_10529982" href="http://apk.neters.club/api/Blog/GoUrl?id=130">【Ids4实战】最全的 v4 版本升级指南</a></li>
<li><a id="post_title_link_10660403" href="http://apk.neters.club/api/Blog/GoUrl?id=74">【Ids4实战】深究配置——用户信息操作篇</a></li>
<li><a id="post_title_link_10911438" href="http://apk.neters.club/api/Blog/GoUrl?id=73">【实战 Ids4】║ 认证中心之内部加权</a></li>
<li><a class="entry" href="http://apk.neters.club/api/Blog/GoUrl?id=72" target="_blank">【实战 Ids4】║ 控制台密码模式搭配Ocelot网关</a></li>
<li><a class="entry" href="http://apk.neters.club/api/Blog/GoUrl?id=71" target="_blank">【实战 Ids4】║ 在Swagger中调试认证授权中心</a>&nbsp;</li>
<li><a class="entry" href="http://apk.neters.club/api/Blog/GoUrl?id=70" target="_blank">【实战 Ids4】║ 又一个项目迁移完成（MVC）</a>&nbsp;</li>
<li></li>
</ul>

```
```



**************************************************************

  技术：

      * .Net Core 3.1 MVC
      
      * EntityFramework Core
      
      * SqlServer/Mysql

      * IdentityServer4

      * Authentication and Authorization

      * OAuth2 and OpenId Connect

      * GrantTypes.Implicit

      * oidc-client
      
      



 
