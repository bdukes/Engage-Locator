<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="AttributeDefinitions.ascx.cs"
    Inherits="Engage.Dnn.Locator.AttributeDefinitions" %>
<%@ Register TagPrefix="dnn" Namespace="DotNetNuke.UI.WebControls" Assembly="DotNetNuke" %>

<div class="adWrapper">
<div class="globalNav">
    <asp:ImageButton ID="lbSettings" CssClass="CommandButton" runat="server" AlternateText="Settings" ImageUrl="~/desktopmodules/EngageLocator/images/settingsBt.gif" OnClick="lbSettings_OnClick" CausesValidation="false" />
    <asp:ImageButton ID="lbManageLocations" CssClass="CommandButton" runat="server" AlternateText="Manage Locations" ImageUrl="~/desktopmodules/EngageLocator/images/locationbt.gif" OnClick="lbManageLocations_OnClick" CausesValidation="false" />
    <asp:ImageButton ID="lbImportFile" CssClass="CommandButton" runat="server" AlternateText="Import File" ImageUrl="~/desktopmodules/EngageLocator/images/importbt.gif" OnClick="lbImportFile_OnClick" CausesValidation="false" />
    <asp:ImageButton ID="lbManageComments" CssClass="CommandButton" runat="server" AlternateText="Comments" ImageUrl="~/desktopmodules/EngageLocator/images/commentsbt.gif" OnClick="lbManageComments_OnClick" CausesValidation="false" />
    <asp:ImageButton ID="lbLocationTypes" CssClass="CommandButton" runat="server" AlternateText="Location Types" OnClick="lbManageTypes_OnClick" ImageUrl="~/desktopmodules/EngageLocator/images/locationTypesBt.gif" CausesValidation="false" />
</div>
<asp:Label ID="lblLocationTypeHelp" runat="Server" class="locatorInstruction Normal" resourcekey="LocationTypeAttributesHelp" />
<asp:UpdatePanel ID="upDataImport" runat="server">
    <ContentTemplate>
        <fieldset class="ltWrapper">
            <legend class="Head" runat="server" id="lgTypes">Location Types</legend>
            <div class="locType">
                <h3 class="SubHead" runat="server" id="hdSelect">Select a Location</h3>
                <div class="locTypeWrapper">
                    <div class="locTypeListView">
                        <asp:ListBox ID="lbLocType" CssClass="Normal" runat="server" Rows="5" Width="100%"
                            AutoPostBack="True" OnSelectedIndexChanged="lbLocType_SelectedIndexChanged">
                        </asp:ListBox>
                    </div>
                    <div class="locTypeNav">
                        <div class="adEditBt">
                            <asp:ImageButton ID="btnEditLocationType" runat="server" ToolTip="Click here to edit this location type"
                                AlternateText="Edit this location type" CssClass="CommandButton" ImageUrl="~/desktopmodules/EngageLocator/images/caEdit.gif"
                                OnClick="btnEditLocationType_Click" />
                        </div>
                        <div class="adDeleteBt">
                            <asp:ImageButton ID="btnDeleteLocationType" runat="server" ToolTip="Click here to delete this location type"
                                AlternateText="Delele this location type" CssClass="CommandButton" 
                                ImageUrl="~/desktopmodules/EngageLocator/images/caDelete.gif" 
                                onclick="btnDeleteLocationType_Click" />
                        </div>
                        <div class="adCreateNewBt">
                            <asp:ImageButton ID="btnCreateLocationType" runat="server" ToolTip="Click here to create a new location type"
                                AlternateText="Create new location type" CssClass="CommandButton" ImageUrl="~/desktopmodules/EngageLocator/images/caCreateNew.gif"
                                OnClick="btnCreateLocationType_Click" />
                        </div>
                        <div id="dvLocationType" runat="server" visible="false">
                            <div id="dvLocationTypeEdit" runat="server">
                                <div class="adAddNewLT">
                                    <div><asp:TextBox ID="txtEditLocationType" runat="server" Width="150px" CssClass="NormalTextBox"></asp:TextBox></div>
                                    <div class="adError"><asp:RequiredFieldValidator ID="rfvName" runat="server" ControlToValidate="txtEditLocationType" CssClass="NormalRed" Display="Dynamic" ErrorMessage="Name is required."></asp:RequiredFieldValidator></div>
                                </div>
                                <div>
                                <asp:ImageButton ID="btnUpdateLocationType" runat="server" ImageUrl ="~/desktopmodules/EngageLocator/images/submitBt.gif" OnClick="btnUpdateLocationType_Click" CssClass="CommandButton" ToolTip="Click here to update changes when done" AlternateText="Submit Changes"></asp:ImageButton>
                                <asp:ImageButton ID="btnCancelLocationType" runat="server" ImageUrl="~/desktopmodules/EngageLocator/images/cancelBt.gif" OnClick="btnCancelLocationType_Click" CssClass="CommandButton" CausesValidation="false" ToolTip="Click here to abort changes" AlternateText="Cancel changes" ></asp:ImageButton>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </fieldset>
        <fieldset class="caWrapper">
            <legend class="Head" id="lgCustomAttributes" runat="server">Custom Attributes</legend>
            <div id="divCustomAttributes" runat="server" class="caData">
                <asp:DataGrid ID="grdLocationTypeAttributes" AutoGenerateColumns="false" runat="server"
                    Width="100%" CellPadding="4" GridLines="None" CssClass="DataGrid_Container" runat="server"
                    OnItemCommand="grdLocationTypeAttributes_ItemCommand" OnItemCreated="grdLocationTypeAttributes_ItemCreated"
                    OnItemDataBound="grdLocationTypeAttributes_ItemDataBound">
                    <HeaderStyle CssClass="NormalBold" VerticalAlign="Top" HorizontalAlign="Center" />
                    <ItemStyle CssClass="DataGrid_Item" HorizontalAlign="Left" />
                    <AlternatingItemStyle CssClass="DataGrid_AlternatingItem" />
                    <EditItemStyle CssClass="NormalTextBox" />
                    <SelectedItemStyle CssClass="NormalRed" />
                    <FooterStyle CssClass="DataGrid_Footer" />
                    <PagerStyle CssClass="DataGrid_Pager" />
                    <Columns>
                        <asp:TemplateColumn>
                            <ItemTemplate>
                                <asp:Label ID="lblId" runat="server" Visible="false" Text='<%# DataBinder.Eval(Container.DataItem, "AttributeDefinitionId").ToString() %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>
                        <dnn:ImageCommandColumn CommandName="Edit" Text="Edit" ImageURL="~/images/edit.gif"
                            HeaderText="Edit" KeyField="AttributeDefinitionID" EditMode="URL" />
                        <dnn:ImageCommandColumn CommandName="Delete" Text="Delete" ImageURL="~/images/delete.gif"
                            HeaderText="Del" KeyField="AttributeDefinitionID" Visible="false" />
                        <dnn:ImageCommandColumn CommandName="MoveDown" ImageURL="~/images/dn.gif" HeaderText="Dn"
                            KeyField="AttributeDefinitionID" />
                        <dnn:ImageCommandColumn CommandName="MoveUp" ImageURL="~/images/up.gif" HeaderText="Up"
                            KeyField="AttributeDefinitionID" />
                        <dnn:TextColumn DataField="AttributeName" HeaderText="Name" Width="100px" HeaderStyle-HorizontalAlign="Left" />
                        <%--		<dnn:textcolumn DataField="AttributeCategory" HeaderText="Category" Width="100px" />--%>
                       <%-- <asp:TemplateColumn HeaderText="DataType">
                            <ItemStyle Width="100px"></ItemStyle>
                            <ItemTemplate>
                                <asp:Label ID="lblDataType" runat="server" Text='<%# DisplayDataType((Engage.Dnn.Locator.AttributeDefinition)Container.DataItem) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateColumn>--%>
                       <%-- <dnn:TextColumn DataField="Length" HeaderText="Length" />--%>
                        <dnn:TextColumn DataField="DefaultValue" HeaderText="DefaultValue" Width="100px" HeaderStyle-HorizontalAlign="Left"/>
                        <%--                        <dnn:TextColumn DataField="ValidationExpression" HeaderText="ValidationExpression"
                            Width="100px" />
                        <dnn:CheckBoxColumn DataField="Required" HeaderText="Required" AutoPostBack="True" />
                        <dnn:CheckBoxColumn DataField="Visible" HeaderText="Visible" AutoPostBack="True" />
--%>
                    </Columns>
                </asp:DataGrid>
            </div>
            <div class="caAddNew">
                <asp:ImageButton ID="btnCAAdd" runat="server" ToolTip="Click here to add a new custom attribute"
                    AlternateText="Add a new custom attribute" CssClass="CommandButton" ImageUrl="~/desktopmodules/EngageLocator/images/caAddNew.gif"
                    OnClick="btnCAAdd_Click" />
            </div>
        </fieldset>
    </ContentTemplate>
</asp:UpdatePanel>
<div class="caNavBt">
    <asp:ImageButton ID="cmdUpdate" runat="server" ToolTip="Click here to update this window"
        AlternateText="Update this entire window" CssClass="CommandButton" ImageUrl="~/desktopmodules/EngageLocator/images/update.gif"
        OnClick="cmdUpdate_Click" />
    <asp:ImageButton ID="cmdCancel" runat="server" ToolTip="Click here to go back to the previous screen"
        AlternateText="Cancel and return to previous screen" CssClass="CommandButton"
        ImageUrl="~/desktopmodules/EngageLocator/images/back.gif" OnClick="cmdCancel_Click" />
</div>
</div>
