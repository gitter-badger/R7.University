﻿using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using DotNetNuke.UI.UserControls;
using DotNetNuke.UI.WebControls;
using DotNetNuke.Web.UI.WebControls;
using R7.University;

namespace R7.University.Division
{
	public partial class EditDivision
	{
		protected ModuleAuditControl ctlAudit;
		protected LinkButton buttonUpdate;
		protected LinkButton buttonDelete;
		protected HyperLink linkCancel;

		protected LabelControl lblTitle;
		protected TextBox txtTitle;
		protected LabelControl lblShortTitle;
		protected TextBox txtShortTitle;

		protected LabelControl lblParentDivision;
		protected DnnComboBox comboParentDivisions;
		protected LabelControl lblDivisionTerm;
		protected DnnTreeView treeDivisionTerms;

		protected LabelControl lblHomePage;
		protected UrlControl urlHomePage;
		protected DnnPageDropDownList pageHomePage;
		protected LabelControl lblWebSite;
		protected TextBox txtWebSite;
		protected LabelControl lblPhone;
		protected TextBox txtPhone;
		protected LabelControl lblFax;
		protected TextBox txtFax;
		protected LabelControl lblEmail;
		protected TextBox txtEmail;
		protected LabelControl lblSecondaryEmail;
		protected TextBox txtSecondaryEmail;
		protected LabelControl lblLocation;
		protected TextBox txtLocation;
		protected LabelControl lblWorkingHours;
		protected TextBox txtWorkingHours;
	}
}
