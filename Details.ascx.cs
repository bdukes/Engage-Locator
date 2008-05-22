using System;
using System.Data;
using System.Globalization;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Common.Lists;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Localization;
using Engage.Dnn.UserFeedback;
using DataProvider=Engage.Dnn.Locator.Data.DataProvider;

namespace Engage.Dnn.Locator
{
    public partial class Details : ModuleBase
    {
        #region Events

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (CommentModerationEnabled)
            {
                lblCommentSubmitted.Text = Localization.GetString("lblCommentSubmittedModerated", LocalResourceFile);
            }
            else
            {
                lblCommentSubmitted.Text = Localization.GetString("lblCommentSubmitted", LocalResourceFile);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                BindData();
            }

            lblAddComment.Visible = CommentsEnabled;
            upnlRating.Visible = RatingsEnabled;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(Globals.NavigateURL(TabId));
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if(!Page.IsValid) return;

            bool approved = false;
            if(!CommentModerationEnabled) approved = true;

            Location.InsertComment(Convert.ToInt32(lblLocationId.Text), txtComment.Text, txtSubmittedBy.Text, approved);
            txtComment.Text = string.Empty;
            txtSubmittedBy.Text = string.Empty;
            Location location = Location.GetLocation(Convert.ToInt32(lblLocationId.Text));
            rptComments.DataSource = location.GetComments(true);
            rptComments.DataBind();
            lblCommentSubmitted.Visible = true;
        }

        #endregion

        #region Methods

        private void BindData()
        {
            lblLocationId.Text = Request.QueryString["lid"];
            Location location = Location.GetLocation(Convert.ToInt32(lblLocationId.Text));

            lblLocationName.Text = location.Name;
            lnkLocationName.Text = location.Website;
            lnkLocationName.NavigateUrl = location.Website;
            lblLocationDetails.Text = location.LocationDetails;

            if (location.Address != String.Empty && location.Address.Contains(","))
            {
                int length = location.Address.IndexOf(',');
                lblLocationsAddress2.Text = location.Address.Remove(0, length);
            }
            lblLocationsAddress1.Text = location.Address;

            ListController controller = new ListController();
            ListEntryInfo region = controller.GetListEntryInfo(location.RegionId);
            if (!DotNetNuke.Common.Utilities.Null.IsNull(region.Value))
            {
                lblLocationsAddress3.Text = location.City + ", " + region.Value + " " + location.PostalCode;
            }
            // Correct fix? - look into proper International address handling.
            else 
            {
                lblLocationsAddress3.Text = location.City + ", " + location.PostalCode;
            }
            lblPhoneNumber.Text = location.Phone;

            DataTable comments = location.GetComments(true).Tables[0];
            if (comments.Rows.Count > 0)
            {
                rptComments.DataSource = comments;
                rptComments.DataBind();
            }
            else
                rptComments.Visible = false;

            ajaxRating.CurrentRating = (int)Math.Round(location.AverageRating);

            foreach (Attribute a in location.GetAttributes())
            {
                Literal lit = new Literal();
                lit.Text = "<div class=div_CustomAttribute" + a.AttributeId.ToString() + ">";
                plhCustomAttributes.Controls.Add(lit);

                Label l = new Label();
                string text = Localization.GetString(a.AttributeName, LocalResourceFile);
                if (text == null)
                    text = a.AttributeName;
                l.Text = text;
                plhCustomAttributes.Controls.Add(l);

                lit = new Literal();
                lit.Text = "&nbsp;";
                plhCustomAttributes.Controls.Add(lit);


                l = new Label();
                l.Text = a.AttributeValue;
                plhCustomAttributes.Controls.Add(l);

                lit = new Literal();
                lit.Text = "<br />";
                plhCustomAttributes.Controls.Add(lit);

                lit = new Literal();
                lit.Text = "</div>";
                plhCustomAttributes.Controls.Add(lit);
            }
        }

        #endregion

        #region Properties

        protected bool ShowLocationDetails
        {
            get
            {
                
                bool showDetails = false;
                ModuleController controller = new ModuleController();
                if (controller.GetTabModuleSettings(TabModuleId)["ShowLocationDetails"] != null)
                {
                    if (controller.GetTabModuleSettings(TabModuleId)["ShowLocationDetails"].ToString() == "DetailsPage")
                        showDetails = true;
                }
                return showDetails;
            }
        }

        #endregion

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Member", Justification = "Controls use lower case prefix")]
        protected void ajaxRating_Changed(object sender, AjaxControlToolkit.RatingEventArgs e)
        {
            Rating.AddRating(Convert.ToInt32(Request.QueryString["lid"], CultureInfo.InvariantCulture), UserId == -1 ? null : (int?)UserId, int.Parse(e.Value, CultureInfo.InvariantCulture), DataProvider.ModuleQualifier);
            ajaxRating.ReadOnly = true;
        }
    }
}