
![Logo](https://github.com/anjoy8/Blog.IdentityServer/blob/master/Blog.IdentityServer/wwwroot/logofull.png)

&nbsp;
&nbsp;

## 给个星星! ⭐️
如果你喜欢这个项目或者它帮助你, 请给 Star~（辛苦星咯）

*********************************************************

## Tips：
```
 /*
  * mysql和sqlserver的迁移操作步骤一致，不过本项目已经迁移好，在Data文件夹下：
  * msql使用MigrationsMySql文件夹下的迁移记录，卸载另一个文件夹
  * sqlserver使用Migrations文件夹下的迁移记录，卸载另一个文件夹
  * 
  * 当然你也可以都删掉，自己重新做迁移。
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
      
      



 
