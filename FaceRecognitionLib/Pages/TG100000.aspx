<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="TG100000.aspx.cs" Inherits="Page_TG100000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="TGIntegration.TGIntegrationSetup"
        PrimaryView="MasterView"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="MasterView" Width="100%" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True" LabelsWidth = "XXL" ControlSize="XXL" />
                        <px:PXTextEdit ID="edFaceApiSubscriptionKey" runat="server" DataField="FaceApiSubscriptionKey" />
                        <px:PXTextEdit ID="edFaceApiEndpoint" runat="server" DataField="FaceApiEndpoint" />
                        <px:PXTextEdit ID="edFaceApiGroupID" runat="server" DataField="FaceApiGroupID" />
                        <px:PXNumberEdit ID="edFaceApiConfidenceThreshold" runat="server" DataField="FaceApiConfidenceThreshold" />
                        <px:PXTextEdit ID="edMapQuestApiKey" runat="server" DataField="MapQuestApiKey" />
		</Template>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" />
	</px:PXFormView>
</asp:Content>