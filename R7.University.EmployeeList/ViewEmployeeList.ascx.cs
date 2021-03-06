﻿using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Linq;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Icons;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.FileSystem;
using R7.University;

namespace R7.University.EmployeeList
{
	public partial class ViewEmployeeList : EmployeeListPortalModuleBase, IActionable
	{
		#region Properties

		protected string EditIconUrl
		{
			get { return IconController.IconURL ("Edit"); }
		}

		#endregion

		#region Handlers

		/*
		/// <summary>
		/// Handles Init event for a control
		/// </summary>
		/// <param name="e">Event args.</param>
		protected override void OnInit (EventArgs e)
		{
			base.OnInit (e);
		}*/
		
        protected IEnumerable<EmployeeAchievementInfo> CommonTitleAchievements;

        protected IEnumerable<OccupiedPositionInfoEx> CommonOccupiedPositions;

		/// <summary>
		/// Handles Load event for a control
		/// </summary>
		/// <param name="e">Event args.</param>
		protected override void OnLoad (EventArgs e)
		{
			base.OnLoad (e);
			
			try
			{
				if (!IsPostBack || ViewState.Count == 0) // Fix for issue #23
				{
					if (Cache_OnLoad ()) return;
					
					// REVIEW: Add Employees.LastYearRating field and sorting by it!
					
					// get employees by DivisionID, in edit mode show also non-published employees
					var	items = EmployeeListController.GetObjects<EmployeeInfo> (CommandType.StoredProcedure, 
						            (EmployeeListSettings.IncludeSubdivisions) ? // which SP to use
							"University_GetRecursiveEmployeesByDivisionID" : "University_GetEmployeesByDivisionID", 
						            EmployeeListSettings.DivisionID, EmployeeListSettings.SortType, IsEditable
					            );

					// check if we have some content to display, 
					// otherwise display a message for module editors or hide module from regular users
					if (!items.Any ())
					{
						// set container control visibility to common users
						Cache_SetContainerVisible (false);
						
						if (IsEditable)
							Utils.Message (this, "NothingToDisplay.Text", MessageType.Info, true);
						else
							// hide entire module
							ContainerControl.Visible = false;
					}
					else
					{
				        var employeeIds = Utils.FormatList (", ", items.Select (em => em.EmployeeID));

                        // get title achievements for all selected employees
                        // TODO: Use {databaseOwner} and {objectQualifier} 
                        CommonTitleAchievements = EmployeeListController.GetObjects<EmployeeAchievementInfo> (CommandType.Text, 
                            string.Format ("SELECT * FROM dbo.vw_University_EmployeeAchievements WHERE [EmployeeID] IN ({0}) AND [IsTitle] = 1", 
                                employeeIds)
                        );

                        // get occupied positions for all selected employees
                        // NOTE: Current division positions go first, then checks IsPrime, then PositionWeight
                        // REVIEW: add "AND [DivisionID] = @1" to display employee positions only from current division
                        CommonOccupiedPositions = EmployeeListController.GetObjects<OccupiedPositionInfoEx> (
                            string.Format ("WHERE [EmployeeID] IN ({0}) ORDER BY (CASE WHEN [DivisionID]={1} THEN 0 ELSE 1 END), [IsPrime] DESC, [PositionWeight] DESC", 
                                employeeIds, EmployeeListSettings.DivisionID)
                        );

                        // set container control visibility to common users
                        Cache_SetContainerVisible (true);

						// bind the data
						listEmployees.DataSource = items;
						listEmployees.DataBind ();
					}
				}
			}
			catch (Exception ex)
			{
				Exceptions.ProcessModuleLoadException (this, ex);
			}
		}

		#endregion

		#region IActionable implementation

		public DotNetNuke.Entities.Modules.Actions.ModuleActionCollection ModuleActions
		{
			get
			{
				// create a new action to add an item, this will be added 
				// to the controls dropdown menu
				var actions = new ModuleActionCollection ();
				actions.Add (
					GetNextActionID (), 
					Localization.GetString ("AddEmployee.Action", this.LocalResourceFile),
					ModuleActionType.AddContent, 
					"", 
					"", 
					Null.IsNull (EmployeeListSettings.DivisionID) ?
                    // pass division_id to select division in which to add employee
                    Utils.EditUrl (this, "EditEmployee") : Utils.EditUrl (this, "EditEmployee", "division_id", EmployeeListSettings.DivisionID.ToString ()),
					false, 
					DotNetNuke.Security.SecurityAccessLevel.Edit,
					true, 
					false
				);
			
				return actions;
			}
		}

		#endregion

		/// <summary>
		/// Handles the items being bound to the datalist control. In this method we merge the data with the
		/// template defined for this control to produce the result to display to the user
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void listEmployees_ItemDataBound (object sender, System.Web.UI.WebControls.DataListItemEventArgs e)
		{
			// e.Item.DataItem is of EmployeeListInfo class
		    var employee = (EmployeeInfo) e.Item.DataItem;
			
			// find controls in DataList item template
            var linkEdit = (HyperLink) e.Item.FindControl ("linkEdit");
            var imageEdit = (Image) e.Item.FindControl ("imageEdit");
            var imagePhoto = (Image) e.Item.FindControl ("imagePhoto");
            var linkDetails = (HyperLink) e.Item.FindControl ("linkDetails"); 
            var labelFullName = (Label) e.Item.FindControl ("labelFullName");
            var labelAcademicDegreeAndTitle = (Label) e.Item.FindControl ("labelAcademicDegreeAndTitle");
            var labelPositions = (Label) e.Item.FindControl ("labelPositions");
            var labelPhones = (Label) e.Item.FindControl ("labelPhones");
            var linkEmail = (HyperLink) e.Item.FindControl ("linkEmail");
            var linkSecondaryEmail = (HyperLink) e.Item.FindControl ("linkSecondaryEmail");
            var linkWebSite = (HyperLink) e.Item.FindControl ("linkWebSite");
            var linkUserProfile = (HyperLink) e.Item.FindControl ("linkUserProfile");

			// edit link
			if (IsEditable)
			{
				if (Null.IsNull (EmployeeListSettings.DivisionID))
					linkEdit.NavigateUrl = Utils.EditUrl (this, "EditEmployee", "employee_id", employee.EmployeeID.ToString ());
				else
					linkEdit.NavigateUrl = Utils.EditUrl (this, "EditEmployee", "employee_id", employee.EmployeeID.ToString (), 
						"division_id", EmployeeListSettings.DivisionID.ToString ());
			}

			// make edit link visible in edit mode
			linkEdit.Visible = IsEditable;
			imageEdit.Visible = IsEditable;
            
			// mark non-published employees, as they visible only to editors
            if (!employee.IsPublished)
            {
                if (e.Item.ItemType == ListItemType.Item)
                    e.Item.CssClass = listEmployees.ItemStyle.CssClass + " _nonpublished";
                else
                    e.Item.CssClass = listEmployees.AlternatingItemStyle.CssClass + " _nonpublished";
            }

			// fill the controls

            // employee photo
            EmployeePhotoLogic.Bind (employee, imagePhoto, EmployeeListSettings.PhotoWidth, true);

			// photo fallback
			if (string.IsNullOrWhiteSpace (imagePhoto.ImageUrl))
			{
				linkDetails.Visible = false;
			}
			else
			{
				// link to employee details
				linkDetails.NavigateUrl = Utils.EditUrl (this, "EmployeeDetails", "employee_id", employee.EmployeeID.ToString ()).Replace ("550,950", "450,950");
			}

			// employee fullname
			labelFullName.Text = employee.FullName;

			/* // Old academic degree and title
			var degreeAndTitle = Utils.FormatList (", ", employee.AcademicDegree, employee.AcademicTitle);
			if (!string.IsNullOrWhiteSpace (degreeAndTitle))
				labelAcademicDegreeAndTitle.Text = "&nbsp;&ndash; " + degreeAndTitle;
			else
				labelAcademicDegreeAndTitle.Visible = false;
			*/

			// get current employee title achievements
			var achievements = CommonTitleAchievements.Where (ach => ach.EmployeeID == employee.EmployeeID);

			var titles = achievements.Select (ach => Utils.FirstCharToLower(ach.DisplayShortTitle)).ToList ();
			
			// add academic degree and title for backward compatibility
			titles.Add (employee.AcademicDegree);
			titles.Add (employee.AcademicTitle);
	
            // employee title achievements
			var strTitles = Utils.FormatList (", ", titles);
			if (!string.IsNullOrWhiteSpace (strTitles))
				labelAcademicDegreeAndTitle.Text = "&nbsp;&ndash; " + strTitles;
			else
				labelAcademicDegreeAndTitle.Visible = false;
			
			// phones
			var phones = Utils.FormatList (", ", employee.Phone, employee.CellPhone);
			if (!string.IsNullOrWhiteSpace (phones))
				labelPhones.Text = phones;
			else
				labelPhones.Visible = false;

			// email
			if (!string.IsNullOrWhiteSpace (employee.Email))
			{
				linkEmail.NavigateUrl = "mailto:" + employee.Email;
				linkEmail.Text = employee.Email; 
			}
			else
				linkEmail.Visible = false;

			// secondary email
			if (!string.IsNullOrWhiteSpace (employee.SecondaryEmail))
			{
				linkSecondaryEmail.NavigateUrl = "mailto:" + employee.SecondaryEmail;
				linkSecondaryEmail.Text = employee.SecondaryEmail; 
			}
			else
				linkSecondaryEmail.Visible = false;

			// webSite
			if (!string.IsNullOrWhiteSpace (employee.WebSite))
			{
				linkWebSite.NavigateUrl = employee.FormatWebSiteUrl;
				linkWebSite.Text = employee.FormatWebSiteLabel;
			}
			else
				linkWebSite.Visible = false;

			// profile link
			if (!Utils.IsNull<int> (employee.UserID))
			{
				linkUserProfile.NavigateUrl = Globals.UserProfileURL (employee.UserID.Value);
				// TODO: Replace profile text with something more sane
				linkUserProfile.Text = Localization.GetString ("VisitProfile.Text", LocalResourceFile);
			}
			else
				linkUserProfile.Visible = false;

			// get current employee occupied positions
			var ops = CommonOccupiedPositions.Where (op => op.EmployeeID == employee.EmployeeID);

			// build positions value
			var positionsVisible = false;
			if (ops != null && ops.Any ())
			{
				var strOps = string.Empty;
				foreach (var op in OccupiedPositionInfoEx.GroupByDivision (ops))
				{
                    var strOp = PositionInfo.FormatShortTitle (op.PositionTitle, op.PositionShortTitle);

					// op.PositionShortTitle is a comma-separated list of positions, including TitleSuffix
					strOps = Utils.FormatList ("; ", strOps, Utils.FormatList (": ", strOp, 
                        // do not display division title also for current division
                        (op.DivisionID != EmployeeListSettings.DivisionID) ? op.FormatDivisionLink (this) : string.Empty));
				}

				if (!string.IsNullOrWhiteSpace (strOps))
				{
					labelPositions.Text = strOps;
					positionsVisible = true;
				}
			}
			labelPositions.Visible = positionsVisible;
		}
	}
}

