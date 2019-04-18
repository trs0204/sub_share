﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace sub_db
{
	internal class c_Languages_
	{
		internal class c_Language
		{
			internal string						m_filename	= "";	// 语言模板文件

			internal int						m_codepage	= 0;	// CodePage
			internal string						m_name		= "";	// 语言名称

			// 文本列表
			internal Dictionary<int, string>	m_txt		= new Dictionary<int, string>();
		};

		static List<c_Language>	m_s_LanguagesList			= new List<c_Language>();
		static int				m_s_current_language_idx	= 0;

		/*==============================================================
		 * 读取语言列表
		 *==============================================================*/
		internal static void	read_languages_list()
		{
			if(!Directory.Exists(c_Path_.m_k_LANGUAGE_DIR))
				return;

			string[] files = Directory.GetFiles(c_Path_.m_k_LANGUAGE_DIR,
												"*.txt",
												SearchOption.TopDirectoryOnly);

			m_s_LanguagesList.Clear();

			foreach(string file in files)
			{
				c_Language language = new c_Language();
				language.m_filename = file;

				m_s_LanguagesList.Add(language);

				string[] lines = File.ReadAllLines(file, Encoding.UTF8);

				foreach(string line in lines)
				{
					if(line.Length < 2)
						continue;

					if(line[0] == '/' && line[1] == '/')
						continue;

					int idx = line.IndexOf(":");
					if(idx < 0)
						continue;

					string w1 = line.Substring(0, idx).Trim().ToLower();
					string w2 = line.Substring(idx + 1).Trim();

					if(w1 == "codepage")
					{
						int.TryParse(w2, out language.m_codepage);
						continue;
					}

					if(w1 == "name")
					{
						language.m_name = w2;
						continue;
					}

					int num;
					if(int.TryParse(w1, out num))
						language.m_txt[num] = w2.Trim();
				}	// for
			}	// for

			// 更新 UI
			c_Mainform.m_s_mainform.toolStripSplitButton_Languages.DropDownItems.Clear();

			for(int i=0; i<m_s_LanguagesList.Count; ++i)
			{
				c_Language lang = m_s_LanguagesList[i];

				ToolStripMenuItem item = new ToolStripMenuItem(lang.m_name);
				item.Tag = i;

				item.Click += Language_Click;

				c_Mainform.m_s_mainform.toolStripSplitButton_Languages.DropDownItems.Add(item);
			}	// for
		}

		/*==============================================================
		 * 改变语言的事件
		 *==============================================================*/
		internal static void Language_Click(object sender, EventArgs e)
		{
			ToolStripMenuItem item = (ToolStripMenuItem)sender;

			int index = (int)item.Tag;

			change_language(index);
		}

		/*==============================================================
		 * 读取语言列表
		 *==============================================================*/
		internal static void	change_language(int index)
		{
			var mainform	= c_Mainform.m_s_mainform;
			var about		= c_Mainform.m_s_mainform.m_About;
			var setting		= c_Mainform.m_s_mainform.m_Setting;
			var update_db	= c_Mainform.m_s_mainform.m_UpdateDatabase;

			((ToolStripMenuItem)mainform.toolStripSplitButton_Languages.DropDownItems[m_s_current_language_idx]).Checked = false;
			((ToolStripMenuItem)mainform.toolStripSplitButton_Languages.DropDownItems[index]).Checked = true;

			m_s_current_language_idx = index;

			// 托盘图标
			mainform.toolStripButton_UpdateDB.Text			= txt(10);	// 更新数据库
			mainform.toolStripSplitButton_Languages.Text	= txt(11);	// 语言(Languages)
			mainform.toolStripButton_Settings.Text			= txt(12);	// 设置
			mainform.toolStripButton_About.Text				= txt(13);	// 关于

			// mainform
			mainform.label_Filter.Text			= txt(20);	// 查询语句：

			// about
			about.Text							= string.Format("{0:s} {1:s} {2:s}",
																txt(13),	// 关于
																c_Common_.m_k_PROGRAM_NAME,
																c_Common_.m_k_VERSION);

			about.label_ChangeLog.Text			= string.Format("{0:s} {1:s}",
																c_Common_.m_k_PROGRAM_NAME,
																txt(30));	// 更新日志

			about.button_OK.Text				= txt(0);	// 确定

			// setting
			setting.Text						= txt(12);	// 设置
			setting.label_subs_path.Text		= txt(40);	// 字幕路径：
			setting.button_OK.Text				= txt(0);	// 确定

			// update_database
			c_Forms_.m_s_Tooltip.SetToolTip(update_db.pictureBox_Start,	txt(50));	// 开始更新数据库
			c_Forms_.m_s_Tooltip.SetToolTip(update_db.pictureBox_Stop,	txt(51));	// 停止更新数据库
			update_db.columnHeader_Time.Text	= txt(52);	// 时间
			update_db.columnHeader_Log.Text		= txt(53);	// 日志
		}

		/*==============================================================
		 * 设置默认语言
		 *==============================================================*/
		internal static void	change_language_to_default()
		{
			int idx = -1;

			for(int i=0; i<m_s_LanguagesList.Count; ++i)
			{
				c_Language lang = m_s_LanguagesList[i];

				if(lang.m_codepage == Encoding.Default.CodePage)
				{
					idx = i;
					break;
				}
			}	// for

			if(idx < 0)
				idx = 2;

			change_language(idx);
		}

		/*==============================================================
		 * 获取文本
		 *==============================================================*/
		internal static string	txt(int num)
		{
			if(m_s_current_language_idx < m_s_LanguagesList.Count)
			{
				var txt_list = m_s_LanguagesList[m_s_current_language_idx].m_txt;

				if(txt_list.ContainsKey(num))
					return txt_list[num];
			}

			return "(no msg)";
		}
	}
}	// namespace sub_db
