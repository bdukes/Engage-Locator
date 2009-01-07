// <copyright file="MainDisplay.ascx.cs" company="Engage Software">
// Engage: Locator - http://www.engagemodules.com
// Copyright (c) 2004-2008
// by Engage Software ( http://www.engagesoftware.com )
// </copyright>
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.

namespace Engage.Dnn.Locator
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Text;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using Data;
    using DotNetNuke.Common;
    using DotNetNuke.Common.Lists;
    using DotNetNuke.Entities.Portals;
    using DotNetNuke.Services.Exceptions;
    using DotNetNuke.Services.Localization;
    using Framework.Templating;
    using Maps;

    /// <summary>
    /// The main display for Engage: Locator.  Display the location search, list, and results.
    /// </summary>
    public partial class MainDisplay : ModuleBase
    {
        /// <summary>
        /// Gets a value indicating whether to allow the user to restrict their search by country.
        /// </summary>
        /// <value><c>true</c> if the search can be restricted by country; otherwise, <c>false</c>.</value>
        protected bool ShowCountry
        {
            get { return Dnn.Utility.GetBoolSetting(this.Settings, "Country", false); }
        }

        /// <summary>
        /// Gets the location details setting.  Possible values include "True," "False," and "SamePage."
        /// </summary>
        /// <value>How to display the location details.</value>
        protected string ShowLocationDetails
        {
            get { return Dnn.Utility.GetStringSetting(this.Settings, "ShowLocationDetails", "False"); }
        }

        /// <summary>
        /// Gets a value indicating whether to allow the user to choose a radius when searching.
        /// </summary>
        /// <value><c>true</c> if the user can choose a search radius; otherwise, <c>false</c>.</value>
        protected bool ShowRadius
        {
            get { return Dnn.Utility.GetBoolSetting(this.Settings, "Radius", true); }
        }

        /// <summary>
        /// Gets the ID of the page on which search results should be displayed
        /// </summary>
        /// <value>The display tab ID.</value>
        private int DisplayTabId
        {
            get { return Dnn.Utility.GetIntSetting(this.Settings, "DisplayResultsTab", this.TabId); }
        }

        /// <summary>
        /// Gets a value indicating whether the default view for this module is to show a list of locations.
        /// </summary>
        /// <value><c>true</c> if this module should show a list of locations by default; otherwise, <c>false</c>.</value>
        private bool ShowDefaultDisplay
        {
            get { return Dnn.Utility.GetBoolSetting(this.Settings, "ShowDefaultDisplay", false); }
        }

        /// <summary>
        /// Gets a value indicating whether to show the map by default.
        /// </summary>
        /// <value>
        /// <c>true</c> if the default display for this module is to show the map; otherwise, <c>false</c>.</value>
        private bool ShowMapDefaultDisplay
        {
            get { return Dnn.Utility.GetBoolSetting(this.Settings, "ShowMapDefaultDisplay", false); }
        }

        /// <summary>
        /// Gets the type of the map.
        /// </summary>
        /// <value>The type of the map.</value>
        private MapType MapType
        {
            get { return Dnn.Utility.GetEnumSetting(this.Settings, "MapType", MapType.Normal); }
        }

        /// <summary>
        /// Gets the number of locations to display on one page of location listings.
        /// </summary>
        /// <value>The size of the page.</value>
        private int PageSize
        {
            get { return Dnn.Utility.GetIntSetting(this.Settings, "LocationsPerPage", 10); }
        }

        /// <summary>
        /// Gets the index of the page.
        /// </summary>
        /// <value>The index of the page.</value>
        private int PageIndex
        {
            get
            { 
                int pageIndex;
                if (int.TryParse(this.Request.QueryString["PageIndex"], NumberStyles.Integer, CultureInfo.InvariantCulture, out pageIndex))
                {
                    return pageIndex - 1;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets the ID of the country to which search results are to be restricted.
        /// </summary>
        /// <value>The ID of the country for which results should be displayed.</value>
        private int? CountryRestrictId
        {
            get
            {
                int countryId;
                if (int.TryParse(this.Request.QueryString["CountryRestrict"], NumberStyles.Integer, CultureInfo.InvariantCulture, out countryId))
                {
                    return countryId;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the address used for search criteria.
        /// </summary>
        /// <value>The search criteria address.</value>
        private string Address
        {
            get
            {
                return this.Request.QueryString["Address"];
            }
        }

        /// <summary>
        /// Gets the city used for search criteria
        /// </summary>
        /// <value>The search criteria city.</value>
        private string City
        {
            get
            {
                return this.Request.QueryString["City"];
            }
        }

        /// <summary>
        /// Gets the postal code used for search criteria.
        /// </summary>
        /// <value>The search criteria postal code.</value>
        private string PostalCode
        {
            get
            {
                return this.Request.QueryString["Zip"];
            }
        }

        /// <summary>
        /// Gets the ID of the region used for search criteria.
        /// </summary>
        /// <value>The ID of the region used for search criteria.</value>
        private int? RegionId
        {
            get
            {
                int regionId;
                if (int.TryParse(this.Request.QueryString["Region"], NumberStyles.Integer, CultureInfo.InvariantCulture, out regionId))
                {
                    return regionId;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the ID of the country used for search criteria.
        /// </summary>
        /// <value>The ID of the country used for search criteria.</value>
        private int? CountryId
        {
            get
            {
                int countryId;
                if (int.TryParse(this.Request.QueryString["Country"], NumberStyles.Integer, CultureInfo.InvariantCulture, out countryId))
                {
                    return countryId;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether all locations should be displayed.
        /// </summary>
        /// <value><c>true</c> if all locations should be displayed; otherwise, <c>false</c>.</value>
        private bool ShowAll
        {
            get
            {
                bool showAll;
                if (bool.TryParse(this.Request.QueryString["All"], out showAll))
                {
                    return showAll;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the radius in miles in which results should be displayed from the search location.
        /// </summary>
        /// <value>The search radius in miles.</value>
        private int? Radius
        {
            get
            {
                int radius;
                if (int.TryParse(this.Request.QueryString["Distance"], NumberStyles.Integer, CultureInfo.InvariantCulture, out radius))
                {
                    return radius;
                }

                return null;
            }
        }

        /// <summary>
        /// Determines whether the instance with the specified TabModuleId is configured.
        /// </summary>
        /// <param name="tabModuleId">The TabModuleId.</param>
        /// <param name="error">The error message.</param>
        /// <returns>
        /// <c>true</c> if the instance with the specified TabModuleId is configured; otherwise, <c>false</c>.
        /// </returns>
        public static new bool IsConfigured(int tabModuleId, ref string error)
        {
            // TODO: Localize error messages
            Hashtable settings = PortalSettings.GetTabModuleSettings(tabModuleId);
            string mapProvider = Dnn.Utility.GetStringSetting(settings, "DisplayProvider");
            if (string.IsNullOrEmpty(mapProvider))
            {
                error = "No Map Provider Selected.";
                return false;
            }

            string className = MapProviderType.GetMapProviderClassByName(mapProvider);
            string apiKey = Dnn.Utility.GetStringSetting(settings, className + ".ApiKey");
            if (string.IsNullOrEmpty(apiKey))
            {
                error = "No API Key Entered.";
                return false;
            }

            MapProvider mp = MapProvider.CreateInstance(className, apiKey);
            if (!mp.IsKeyValid())
            {
                error = "API Key is not in the correct format.";
                return false;
            }

            if (string.IsNullOrEmpty(Dnn.Utility.GetStringSetting(settings, "DisplayProvider")))
            {
                error = "Search Setting \"Results Display Module\" not set.";
                return false;
            }

            if (string.IsNullOrEmpty(Dnn.Utility.GetStringSetting(settings, "ShowLocationDetails")))
            {
                error = "Display Setting \"Show Location Details\" not set.";
                return false;
            }

            if (!Dnn.Utility.GetIntSetting(settings, "DisplayResultsTabId").HasValue)
            {
                error = "Display Results not set.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string title = this.Settings["SearchTitle"] != null
                                       ? this.Settings["SearchTitle"].ToString()
                                       : Localization.GetString("lblHeader", this.LocalResourceFile);

                this.lblHeader.Text = title;

                if (!this.SubmissionsEnabled)
                {
                    this.lnkSubmitLocation.Visible = false;
                }

                if (!this.SubmissionsEnabled)
                {
                    this.lnkSubmitLocationSearch.Visible = false;
                }

                this.lbSettings.Visible = this.IsEditable;
                this.lbImportFile.Visible = this.IsEditable;
                this.lbManageComments.Visible = this.IsEditable;
                this.lbManageLocations.Visible = this.IsEditable;
                this.lbLocationTypes.Visible = this.IsEditable;

                string error = String.Empty;
                if (!IsConfigured(this.TabModuleId, ref error))
                {
                    this.mvwLocator.SetActiveView(this.vwSetup);
                    if (this.UserInfo.IsInRole(this.PortalSettings.ActiveTab.AdministratorRoles))
                    {
                        // "Please setup module through the Module Settings page.<br>Required Items: <br/><br/>" + error;
                        this.lblSetupText.Text = Localization.GetString("SetupText_Admin", this.LocalResourceFile) + error;
                    }
                    else
                    {
                        // "This page is not configured. Please contact an Administrator.";
                        this.lblSetupText.Text = Localization.GetString("SetupText_NonAdmin", this.LocalResourceFile);
                    }
                }
                else
                {
                    this.mvwLocator.SetActiveView(this.vwLocator);
                    if (!this.IsPostBack)
                    {
                        this.FillDropDowns();

                        if (this.HasSearchCriteria() || this.ShowDefaultDisplay || this.ShowMapDefaultDisplay || this.ShowAll || this.CountryRestrictId.HasValue)
                        {
                            this.DisplayLocationList();
                        }

                        this.SetSearchDisplay();

                        if (!this.ShowCountry && !this.ShowRadius)
                        {
                            this.btn_ShowAll.Visible = false;
                        }

                        if (this.SubmissionsEnabled)
                        {
                            this.lnkSubmitLocation.Visible = true;
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        /// <summary>
        /// Handles the Click event of the btnBack control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnBack_Click(object sender, EventArgs e)
        {
            this.mvwLocator.SetActiveView(this.vwLocator);

            this.txtLocationAddress.Text = string.Empty;
            this.txtLocationCity.Text = string.Empty;
            this.ddlLocationRegion.SelectedIndex = 0;
            this.txtLocationPostalCode.Text = string.Empty;
            this.ddlLocatorCountry.SelectedIndex = 0;
            if (this.ShowCountry)
            {
                this.ddlCountry.SelectedIndex = 0;
                this.ddlDistance.SelectedIndex = 0;
            }
            else
            {
                this.ddlCountry.SelectedValue = Localization.GetString("UnlimitedMiles", this.LocalResourceFile);
            }

            this.pnlError.Visible = false;

            this.divMap.Style[HtmlTextWriterStyle.Display] = "none";
        }

        /// <summary>
        /// Handles the Click event of the btnShowAll control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnShowAll_Click(object sender, EventArgs e)
        {
            this.Response.Redirect(Globals.NavigateURL(this.TabId, string.Empty, "All=" + true.ToString(CultureInfo.InvariantCulture)));
        }

        /// <summary>
        /// Handles the Click event of the btnSubmit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            List<string> parameters = new List<string>();
            AddParameter(parameters, "Address", this.txtLocationAddress.Visible ? this.txtLocationAddress.Text : null);
            AddParameter(parameters, "City", this.txtLocationCity.Visible ? this.txtLocationCity.Text : null);
            AddParameter(parameters, "Region", this.ddlLocationRegion.Visible ? this.ddlLocationRegion.SelectedValue : null);
            AddParameter(parameters, "Zip", this.txtLocationPostalCode.Visible ? this.txtLocationPostalCode.Text : null);
            AddParameter(parameters, "Country", this.ddlLocatorCountry.Visible ? this.ddlLocatorCountry.SelectedValue : null);
            AddParameter(parameters, "Distance", this.ddlDistance.Visible ? this.ddlDistance.SelectedValue : null);
            AddParameter(parameters, "CountryRestrict", this.ddlCountry.Visible ? this.ddlCountry.SelectedValue : null);

            this.Response.Redirect(Globals.NavigateURL(this.DisplayTabId, string.Empty, parameters.ToArray()));
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ddlCountry control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ddlCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ShowRadius)
            {
                this.ddlDistance.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Handles the SelectedIndexChanged event of the ddlDistance control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void ddlDistance_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.ShowCountry)
            {
                this.ddlCountry.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Handles the Click event of the lnkSubmitLocations control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected void lnkSubmitLocations_Click(object sender, EventArgs e)
        {
            this.Response.Redirect(Globals.NavigateURL(this.TabId, "ManageLocation", "mid=" + this.ModuleId));
        }

        /// <summary>
        /// Handles the ItemDataBound event of the rptLocations control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Web.UI.WebControls.RepeaterItemEventArgs"/> instance containing the event data.</param>
        protected void rptLocations_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                Location location = (Location)e.Item.DataItem;
                Label lblLocationsGridDistance = (Label)e.Item.FindControl("lblLocationsGridDistance");
                Label lblLocationDetails = (Label)e.Item.FindControl("lblLocationDetails");
                Label l = (Label)e.Item.FindControl("lblLocationsGridAddress");

                lblLocationDetails.Visible = (this.ShowLocationDetails == "SamePage" || this.ShowLocationDetails == "True");

                if (location != null)
                {
                    if (this.ShowLocationDetails == "DetailsPage")
                    {
                        HyperLink lnkShowLocationDetails = (HyperLink)e.Item.FindControl("lnkShowLocationDetails");
                        lnkShowLocationDetails.Visible = true;
                        lnkShowLocationDetails.NavigateUrl = Globals.NavigateURL(
                                this.TabId, "Details", "mid=" + this.ModuleId + "&lid=" + location.LocationId);
                        lnkShowLocationDetails.Text = Localization.GetString("lnkShowLocationDetails", this.LocalResourceFile);
                    }

                    lblLocationsGridDistance.Visible = location.Distance.HasValue;

                    if (lblLocationsGridDistance.Visible)
                    {
                        lblLocationsGridDistance.Text = location.Distance.Value.ToString("#.00", CultureInfo.CurrentCulture)
                                                        + Localization.GetString("Miles", this.LocalResourceFile);
                    }

                    HyperLink lnkSite = (HyperLink)e.Item.FindControl("lbSiteLink");
                    lnkSite.Text = string.IsNullOrEmpty(location.Website)
                                           ? Localization.GetString("Closed", this.LocalResourceFile)
                                           : Localization.GetString("Open", this.LocalResourceFile);

                    l.Text = location.FullAddress;
                }
            }
        }

        /// <summary>
        /// Disables the paging link.
        /// </summary>
        /// <param name="pageLink">The page link.</param>
        private static void DisablePagingLink(HyperLink pageLink)
        {
            pageLink.Enabled = false;
            pageLink.CssClass = Engage.Utility.AddCssClass(pageLink.CssClass, "NormalDisabled");
        }

        /// <summary>
        /// Adds the QueryString parameter with the given <paramref name="key"/> to the list of parameters, if it is already on the QueryString.
        /// </summary>
        /// <param name="parameters">The list of parameters.</param>
        /// <param name="key">The key of the parameter in the QueryString.</param>
        /// <param name="value">The value of the parameter in the QueryString.</param>
        private static void AddParameter(ICollection<string> parameters, string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                parameters.Add(key + "=" + HttpUtility.UrlPathEncode(value));
            }
        }

        /// <summary>
        /// Adds the QueryString parameter with the given <paramref name="key"/> to the list of parameters, if it is already on the QueryString.
        /// </summary>
        /// <param name="parameters">The list of parameters.</param>
        /// <param name="key">The key of the value in the QueryString.</param>
        private void AddParameter(ICollection<string> parameters, string key)
        {
            AddParameter(parameters, key, this.Request.QueryString[key]);
        }

        /// <summary>
        /// Configures the paging, setting the links to the previous and next buttons, setting label text, etc.
        /// </summary>
        /// <param name="pagingState">Current state of the paging.</param>
        private void ConfigurePaging(ItemPagingState pagingState)
        {
            if (pagingState.CurrentPage > 1)
            {
                this.PreviousPageLink.NavigateUrl = this.GenerateUrlForPage(pagingState.Previous().CurrentPage);
            }
            else
            {
                DisablePagingLink(this.PreviousPageLink);
            }

            if (pagingState.CurrentPage < pagingState.TotalPages)
            {
                this.NextPageLink.NavigateUrl = this.GenerateUrlForPage(pagingState.Next().CurrentPage);
            }
            else
            {
                DisablePagingLink(this.NextPageLink);
            }

            this.CurrentPageLabel.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    Localization.GetString("CurrentPageLabel.Text", this.LocalResourceFile),
                    pagingState.CurrentPage,
                    pagingState.TotalPages);
        }

        /// <summary>
        /// Generates the URL for the listing.
        /// </summary>
        /// <param name="pageIndex">Index of the page.</param>
        /// <returns>A URL to the given page of this list</returns>
        private string GenerateUrlForPage(int pageIndex)
        {
            List<string> parameters = new List<string>();
            parameters.Add("PageIndex=" + pageIndex.ToString(CultureInfo.InvariantCulture));
            this.AddParameter(parameters, "Address");
            this.AddParameter(parameters, "City");
            this.AddParameter(parameters, "Region");
            this.AddParameter(parameters, "Zip");
            this.AddParameter(parameters, "Country");
            this.AddParameter(parameters, "Distance");

            return Globals.NavigateURL(this.TabId, string.Empty, parameters.ToArray());
        }

        private static string ResolveRegionId(int? regionId)
        {
            return regionId.HasValue ? new ListController().GetListEntryInfo(regionId.Value).Value : null;
        }

        private void DisplayLocationList()
        {
            LocationCollection locations = null;
            MapProvider mapProvider = this.GetMapProvider();

            if (this.CountryRestrictId.HasValue)
            {
                locations = Location.GetLocationsByCountry(this.CountryRestrictId.Value, this.PortalId, this.PageIndex, this.PageSize);
            }
            else if (this.HasSearchCriteria())
            {
                System.Diagnostics.Debug.Fail("Get values from drop downs");
                GeocodeResult geocodeResult = mapProvider.GeocodeLocation(this.Address, this.City, ResolveRegionId(this.RegionId), this.PostalCode);

                if (geocodeResult.Successful)
                {
                    int[] locationTypeIds = Engage.Utility.ParseIntegerList(Dnn.Utility.GetStringSetting(this.Settings, "DisplayTypes").Split(','), CultureInfo.InvariantCulture).ToArray();
                    locations = Location.GetAllLocationsByDistance(geocodeResult.Latitude, geocodeResult.Longitude, this.Radius, this.PortalId, locationTypeIds, this.PageIndex, this.PageSize);
                }
                else
                {
                    this.lblErrorMessage.Text = geocodeResult.ErrorMessage;
                }
            }
            else if (this.ShowDefaultDisplay || this.ShowMapDefaultDisplay || this.ShowAll)
            {
                locations = this.GetDefaultLocations();
            }
            else
            {
                this.lblErrorMessage.Text = Localization.GetString("lblErrorMessage", this.LocalResourceFile);
            }

            if (locations == null || locations.Count < 1)
            {
                this.lblErrorMessage.Text = Localization.GetString("NoLocationsFound", this.LocalResourceFile);
                this.pnlError.Visible = true;
            }
            else
            {
                this.mvwLocator.SetActiveView(this.vwResults);

                this.lblNumClosest.Text = String.Format(CultureInfo.CurrentCulture, Localization.GetString("lblNumClosest", this.LocalResourceFile), locations.TotalRecords);
                this.ConfigurePaging(new ItemPagingState(this.PageIndex + 1, locations.TotalRecords, this.PageSize));

                this.rptLocations.DataSource = locations;
                this.rptLocations.DataBind();

                this.ShowMaps(locations);
            }

            ////this.lnkViewMap.Visible = false;
        }

        /// <summary>
        /// Fills the drop down list of countries.
        /// </summary>
        private void FillCountry()
        {
            // Check if we have any countries yet
            DataTable countriesTable = DataProvider.Instance().GetCountriesList(this.PortalId);
            if (countriesTable.Rows.Count > 0)
            {
                this.ddlCountry.DataSource = countriesTable;
                this.ddlCountry.DataTextField = "Text";
                this.ddlCountry.DataValueField = "EntryId";
                this.ddlCountry.DataBind();
                this.ddlCountry.Items.Insert(0, new ListItem(Localization.GetString("ChooseOne", this.LocalResourceFile), string.Empty));

                ListItem defaultCountryListItem = this.ddlCountry.Items.FindByValue(Dnn.Utility.GetStringSetting(this.Settings, "ShowLocationDetails"));
                if (defaultCountryListItem != null)
                {
                    // remove default country and add it back at the top of the list
                    this.ddlCountry.Items.Remove(defaultCountryListItem);
                    this.ddlCountry.Items.Insert(1, defaultCountryListItem);
                }
            }
        }

        /// <summary>
        /// Fills the drop down lists on the search form.  Also shows or hides them based on the module's settings
        /// </summary>
        private void FillDropDowns()
        {
            this.lblSearchTitle.Text = Dnn.Utility.GetStringSetting(this.Settings, "SearchTitle", Localization.GetString("lblSearchTitle", this.LocalResourceFile));
            this.pnlAddress.Visible = Dnn.Utility.GetBoolSetting(this.Settings, "Address", true);
            this.pnlCountry.Visible = this.ShowCountry;
            this.pnlDistance.Visible = this.ShowRadius;

            foreach (ListItem li in this.ddlDistance.Items)
            {
                // li.Value becomes li.Text if you don't explicitly reset it, as below.  BD
                li.Value = li.Value;
                li.Text = String.Format(CultureInfo.CurrentCulture, "{0} {1}", li.Value, Localization.GetString("Miles", this.LocalResourceFile));
            }

            this.ddlDistance.Items.Add(new ListItem(Localization.GetString("UnlimitedMiles", this.LocalResourceFile), string.Empty));
            this.ddlDistance.SelectedValue = string.Empty;

            ListController listController = new ListController();

            // Load the state list
            this.ddlLocationRegion.DataSource = listController.GetListEntryInfoCollection("Region");
            this.ddlLocationRegion.DataTextField = "Text";
            this.ddlLocationRegion.DataValueField = "EntryID";
            this.ddlLocationRegion.DataBind();
            this.ddlLocationRegion.Items.Insert(0, new ListItem(Localization.GetString("ChooseOne", this.LocalResourceFile), string.Empty));

            // fill the country dropdown
            this.ddlLocatorCountry.DataSource = listController.GetListEntryInfoCollection("Country");
            this.ddlLocatorCountry.DataTextField = "Text";
            this.ddlLocatorCountry.DataValueField = "EntryId";
            this.ddlLocatorCountry.DataBind();
            this.ddlLocatorCountry.Items.Insert(0, new ListItem(Localization.GetString("ChooseOne", this.LocalResourceFile), string.Empty));
            this.ddlLocatorCountry.SelectedIndex = 0;

            if (this.ShowCountry)
            {
                this.FillCountry();

                ////if (this.ShowRadius)
                ////{
                ////    this.ddlDistance.ClearSelection();
                ////    this.ddlDistance.Items.Insert(0, new ListItem(Localization.GetString("ChooseOne", this.LocalResourceFile), string.Empty));
                ////    this.ddlDistance.SelectedIndex = 0;
                ////}
            }
        }

        /// <summary>
        /// Gets the default list of all locations.
        /// </summary>
        /// <returns>The default list of all locations</returns>
        private LocationCollection GetDefaultLocations()
        {
            return Location.GetAllLocationsByType(this.PortalId, Engage.Utility.ParseIntegerList(Dnn.Utility.GetStringSetting(this.Settings, "DisplayTypes").Split(','), CultureInfo.InvariantCulture).ToArray(), this.PageIndex, this.PageSize);
        }

        /// <summary>
        /// Determines whether there is search criteria.
        /// </summary>
        /// <returns>
        /// <c>true</c> if there is search criteria; otherwise, <c>false</c>.
        /// </returns>
        private bool HasSearchCriteria()
        {
            return !string.IsNullOrEmpty(this.Address) || !string.IsNullOrEmpty(this.City)
                   || this.RegionId.HasValue || !string.IsNullOrEmpty(this.PostalCode)
                   || this.CountryId.HasValue;
        }

        /// <summary>
        /// Sets the visibility of the search components, based on this module's settings
        /// </summary>
        private void SetSearchDisplay()
        {
            this.addressFirstLine.Visible = Dnn.Utility.GetBoolSetting(this.Settings, "SearchAddress", false);
            this.ltCity.Visible = this.ltRegion.Visible = Dnn.Utility.GetBoolSetting(this.Settings, "SearchCityRegion", false);
            this.ltPostalcode.Visible = Dnn.Utility.GetBoolSetting(this.Settings, "SearchPostalCode", true);
            this.ltCountry.Visible = Dnn.Utility.GetBoolSetting(this.Settings, "SearchCountry", false);
        }

        /// <summary>
        /// Sets up and shows the map.
        /// </summary>
        /// <param name="locations">The list of locations being displayed.</param>
        private void ShowMaps(LocationCollection locations)
        {
            this.divMap.Visible = /*this.lnkViewMap.Visible =*/ true;

            this.GetMapProvider().RegisterMapScript(
                ScriptManager.GetCurrent(this.Page),
                this.MapType,
                this.divMap.ClientID,
                this.CurrentLocationLabel.ClientID,
                this.lblLocatorMapLabel.ClientID,
                this.lblScrollToViewMore.ClientID,
                this.DrivingDirectionsLink.ClientID,
                this.MapLinkPanel.ClientID,
                locations,
                this.ShowMapDefaultDisplay || this.ShowAll);
        }
    }
}