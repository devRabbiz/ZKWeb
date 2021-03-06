﻿using Microsoft.EntityFrameworkCore;
using System;
using ZKWeb.Server;
using ZKWeb.Storage;

namespace ZKWeb.ORM.EFCore {
	/// <summary>
	/// A base database context only contains migration history table<br/>
	/// 一个数据库上下文的基类, 只包含迁移历史表<br/>
	/// </summary>
	public class EFCoreDatabaseContextBase : DbContext {
		/// <summary>
		/// Database type<br/>
		/// 数据库类型<br/>
		/// </summary>
		private string DatabaseName { get; set; }
		/// <summary>
		/// Connection string<br/>
		/// 连接字符串<br/>
		/// </summary>
		private string ConnectionString { get; set; }

		/// <summary>
		/// Initialize<br/>
		/// 初始化<br/>
		/// </summary>
		/// <param name="database">Database type</param>
		/// <param name="connectionString">Connection string</param>
		public EFCoreDatabaseContextBase(string database, string connectionString) {
			DatabaseName = database;
			ConnectionString = connectionString;
		}

		/// <summary>
		/// Configure context options<br/>
		/// 配置上下文选项<br/>
		/// </summary>
		/// <param name="optionsBuilder">Options builder</param>
		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			var pathConfig = Application.Ioc.Resolve<LocalPathConfig>();
			if (string.Compare(DatabaseName, "MSSQL", true) == 0) {
				optionsBuilder.UseSqlServer(
					ConnectionString, option => option.UseRowNumberForPaging());
			} else if (string.Compare(DatabaseName, "SQLite", true) == 0) {
				optionsBuilder.UseSqlite(
					ConnectionString.Replace("{{App_Data}}", pathConfig.AppDataDirectory));
			} else if (string.Compare(DatabaseName, "MySQL", true) == 0) {
				optionsBuilder.UseMySql(ConnectionString);
			} else if (string.Compare(DatabaseName, "PostgreSQL", true) == 0) {
				optionsBuilder.UseNpgsql(ConnectionString);
			} else if (string.Compare(DatabaseName, "InMemory", true) == 0) {
				optionsBuilder.UseInMemoryDatabase();
			} else {
				throw new ArgumentException($"unsupported database type {Database}");
			}
		}

		/// <summary>
		/// Configure entity model<br/>
		/// 配置实体模型<br/>
		/// </summary>
		/// <param name="modelBuilder">Model builder</param>
		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			new EFCoreMigrationHistory().Configure(modelBuilder);
		}
	}
}
