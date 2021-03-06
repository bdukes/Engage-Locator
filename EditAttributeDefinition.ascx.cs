using Globals = DotNetNuke.Common.Globals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Common.Lists;
using DotNetNuke.Services.Localization;

using DotNetNuke.UI.Utilities;
using DotNetNuke.Common.Utilities;
using System;

namespace Engage.Dnn.Locator
{
    using System.Globalization;

    partial class EditAttributeDefinition : ModuleBase
    {

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            divDelete.Visible = ((bool)IsAddMode == false);
            lgDefinitions.InnerText = Localization.GetString("lgDefinitions", LocalResourceFile);
        }


        #region Private Members

        
        //private string ResourceFile = "~/Admin/Users/App_LocalResources/LocationType.ascx";

        #endregion

        #region Protected Members

        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        protected bool IsAddMode
        {
            get { return Request.QueryString["AttributeDefinitionID"] == null; }
        }

        protected bool IsList
        {
            get
            {
                bool _IsList = false;
                ListController objListController = new ListController();
                ListEntryInfo dataType = objListController.GetListEntryInfo(AttributeDefinition.DataType);

                if (((dataType != null)) && (dataType.ListName == "DataType") && (dataType.Value == "List"))
                {
                    _IsList = true;
                }

                return _IsList;
            }
        }

        protected bool IsSuperUser
        {
            get
            {
                if (PortalSettings.ActiveTab.ParentId == PortalSettings.SuperTabId)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        [System.Diagnostics.DebuggerBrowsable(System.Diagnostics.DebuggerBrowsableState.Never)]
        private AttributeDefinition _attributeDefinition;
        protected AttributeDefinition AttributeDefinition
        {
            get
            {
                //_attributeDefinition = LocationType.GetAttributeDefinition(AttributeDefinitionId, LocationTypeId);
                if (_attributeDefinition == null)
                {
                    if ((bool)IsAddMode)
                    {
                        //Create New Attribute Definition
                        _attributeDefinition = new AttributeDefinition();
                        _attributeDefinition.PortalId = UsersPortalId;
                        _attributeDefinition.LocationTypeId = LocationTypeId;
                        ListController controller = new ListController();
                        ListEntryInfo info = controller.GetListEntryInfo("DataType", "Text");
                        _attributeDefinition.DataType = info.EntryID;
                        _attributeDefinition.AttributeName = txtName.Text;
                        _attributeDefinition.DefaultValue = txtDefaultValue.Text;
                        _attributeDefinition.ViewOrder = LocationType.GetAttributeDefinitions(LocationTypeId).Count;
                    }
                    else
                    {
                        //Get Attribute Definition from Data Store
                        _attributeDefinition = LocationType.GetAttributeDefinition(AttributeDefinitionId, LocationTypeId);
                    }
                }
                return _attributeDefinition;
            }
        }

        protected int AttributeDefinitionId
        {
            get
            {
                int id = Null.NullInteger;
                if (ViewState["AttributeDefinitionID"] != null)
                {
                    id = Int32.Parse(ViewState["AttributeDefinitionID"].ToString(), CultureInfo.InvariantCulture);
                }

                if (Request.QueryString["AttributeDefinitionId"] != null)
                {
                    id = Convert.ToInt32(Request.QueryString["AttributeDefinitionId"], CultureInfo.InvariantCulture);
                }

                return id;
            }

            set
            {
                ViewState["AttributeDefinitionID"] = value;
            }
        }

        protected int LocationTypeId
        {
            get
            {
                int id = Null.NullInteger;
                if (Request.QueryString["ltid"] != null)
                {
                    id = Int32.Parse(this.Request.QueryString["ltid"], CultureInfo.InvariantCulture);
                }

                return id;
            }
        }

        protected int UsersPortalId
        {
            get
            {
                int id = PortalId;
                if (IsSuperUser)
                {
                    id = Null.NullInteger;
                }
                return id;
            }
        }

        #endregion

        #region Event Handlers

        private void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                // Get Attribute Definition Id from Querystring (unless its been stored in the Viewstate)
                if (AttributeDefinitionId == Null.NullInteger)
                {
                    if ((Request.QueryString["AttributeDefinitionId"] != null))
                    {
                        AttributeDefinitionId = Int32.Parse(Request.QueryString["AttributeDefinitionId"], CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        cmdDelete.Visible = false;
                    }
                }


                if (!Page.IsPostBack)
                {
                    // Add Delete Confirmation
                    cmdDelete.Visible = true;
                    ClientAPI.AddButtonConfirm(cmdDelete, Localization.GetString("DeleteAttribute", LocalResourceFile));

                    BindData();
                }

            }

            catch (Exception exc)
            {
                //Module failed to load
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdUpdate_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            try
            {
                AttributeDefinition.AttributeName = txtName.Text;
                AttributeDefinition.DefaultValue = txtDefaultValue.Text;

                if (this.IsAddMode)
                {
                    LocationType.AddAttributeDefinition(AttributeDefinition);
                    Response.Redirect(Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "AttributeDefinitions", "mid=" + ModuleId.ToString(CultureInfo.InvariantCulture) + "&ltid=" + LocationTypeId.ToString(CultureInfo.InvariantCulture)));
                }
                else
                {
                    LocationType.UpdateAttributeDefinition(AttributeDefinition);
                    Response.Redirect(Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "AttributeDefinitions", "mid=" + ModuleId.ToString(CultureInfo.InvariantCulture) + "&ltid=" + LocationTypeId.ToString(CultureInfo.InvariantCulture)));
                }
            }
            catch (ModuleLoadException exc)
            {
                // Module failed to load
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdCancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            try
            {
                Response.Redirect(Globals.NavigateURL(PortalSettings.ActiveTab.TabID, "AttributeDefinitions", "mid=" + ModuleId.ToString(CultureInfo.InvariantCulture) + "&ltid=" + LocationTypeId.ToString(CultureInfo.InvariantCulture)));
            }
            catch (ModuleLoadException exc)
            {
                //Module failed to load
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        protected void cmdDelete_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            try
            {
                if (AttributeDefinitionId != Null.NullInteger)
                {
                    //Delete the Attribute Definition
                    LocationType.DeleteAttributeDefinition(AttributeDefinition);
                }

                //Redirect to Definitions page
                Response.Redirect(EditUrl("AttributeDefinitions"), true);
            }
            catch (ModuleLoadException exc)
            {
                //Module failed to load
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        #endregion

        private void BindData()
        {
            _attributeDefinition = LocationType.GetAttributeDefinition(AttributeDefinitionId, LocationTypeId);
            txtName.Text = AttributeDefinition.AttributeName;
            txtDefaultValue.Text = AttributeDefinition.DefaultValue;
        }

    }
}

