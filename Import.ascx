﻿<%@ Control Language="C#" Inherits="Engage.Dnn.Locator.Import" AutoEventWireup="True"
    CodeBehind="Import.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" %>

<div class="div_ManagementButtons">
    <asp:ImageButton ID="lbSettings" CssClass="CommandButton" runat="server" AlternateText="Settings" ImageUrl="~/desktopmodules/EngageLocator/images/settingsBt.gif" OnClick="lbSettings_OnClick" />
    <asp:ImageButton ID="lbManageLocations" CssClass="CommandButton" runat="server" AlternateText="Manage Locations" ImageUrl="~/desktopmodules/EngageLocator/images/locationbt.gif" OnClick="lblManageLocations_OnClick" />
    <asp:ImageButton ID="lbImportFile" CssClass="CommandButton" runat="server" AlternateText="Import File" ImageUrl="~/desktopmodules/EngageLocator/images/importbt.gif" OnClick="lblImportFile_OnClick" />
    <asp:ImageButton ID="lbManageComments" CssClass="CommandButton" runat="server" AlternateText="Import File" ImageUrl="~/desktopmodules/EngageLocator/images/commentsbt.gif" OnClick="lblManageComments_OnClick" />
    <asp:ImageButton ID="lbLocationTypes" CssClass="CommandButton" runat="server" AlternateText="Location Types" OnClick="lblManageTypes_OnClick" ImageUrl="~/desktopmodules/EngageLocator/images/locationTypesBt.gif" />
</div>
<br />
<asp:Label ID="lblConfigured" runat="server" CssClass="Normal" Text="Module is not Configured. Please go to Module Settings and configure module before managing locations."
    Visible="False" resourcekey="lblConfigured"></asp:Label>
    <fieldset style="width:50%" >
    <legend class="Head" id="lgImport" runat="server">Import File</legend>
    <asp:Label ID="lblInstructions" runat="server" CssClass="Normal" Text="Click the browse button to locate a .csv file to import your locations." resourceKey="lblInstructions"></asp:Label>
    <div class="divPanelTab" id="divPanelTab" runat="server">
        <div class="importPanel" runat="server" id="fileDiv">
            <asp:FileUpload ID="fileImport" runat="server" /><br />
            <asp:Label ID="lblMessage" runat="server" CssClass="Normal"></asp:Label><br />
        </div>
    </fieldset>
<br />
<br />
<div id="div_navigation">
    <asp:ImageButton ID="btnSubmit" runat="server" CssClass="CommandButton" OnClick="btnSubmit_Click"
        ToolTip="Select a file and click Submit to queue your file for import." AlternateText="Submit"
        ImageUrl="~/desktopmodules/EngageLocator/images/submit_bt.gif" />
    <asp:ImageButton ID="btnBack" runat="server" CssClass="CommandButton" OnClick="btnBack_Click"
        ToolTip="Click here to go back to the previous screen." AlternateText="Cancel"
        ImageUrl="~/desktopmodules/EngageLocator/images/back.gif" />
</div>
</div> 