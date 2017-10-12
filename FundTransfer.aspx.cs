using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class FundTransfer : Page
{
    private Customer _transferorCustomer;
    private Customer _transfereeCustomer;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack) return;
        PopulateDropDown();

        pnlConfirm.Visible = false;

        txtTransferOn.Text = Convert.ToString(calTransferDate.TodaysDate.ToString("MM/dd/yyyy"));
    }

    protected override void OnInit(EventArgs e)
    {
        wizFunds.MoveTo(wizStep1);
        base.OnInit(e);
    }

    protected void PopulateDropDown()
    {
        var defaultItem = new ListItem
        {
            Selected = true,
            Value = "-1",
            Text = "Select transferor"
        };

        ddlTransferor.Items.Add(defaultItem);
        ddlTransferee.Items.Add(defaultItem);

        foreach (var listItem in from Customer customer in CustomerDataAccess.getAllCustomers()
            select new ListItem
            {
                Value = customer.Id,
                Text = customer.ToString()
            })
        {
            ddlTransferor.Items.Add(listItem);
            ddlTransferee.Items.Add(listItem);
        }

        ddlTransferor.ClearSelection();
        ddlTransferee.ClearSelection();

    }

    protected void ddlTransferor_SelectedIndexChanged(object sender, EventArgs e)
    {
        _transferorCustomer = CustomerDataAccess.getCustomerById(ddlTransferor.SelectedValue);

        rblFromAccount.Items[0].Value = Convert.ToString(_transferorCustomer.Checking.Balance);
        rblFromAccount.Items[0].Text = "Checking - Balance: " + String.Format("{0:C}", _transferorCustomer.Checking.Balance);

        rblFromAccount.Items[1].Value = Convert.ToString(_transferorCustomer.Saving.Balance);
        rblFromAccount.Items[1].Text = "Saving - Balance: " + String.Format("{0:C}", _transferorCustomer.Saving.Balance);

        CustomRangeValidator(rblFromAccount);
    }

    protected void ddlTransferee_SelectedIndexChanged(object sender, EventArgs e)
    {
        _transfereeCustomer = CustomerDataAccess.getCustomerById(ddlTransferee.SelectedValue);

        rblToAccount.Items[0].Value = Convert.ToString(_transfereeCustomer.Checking.Balance);
        rblToAccount.Items[0].Text = "Checking - Balance: " + String.Format("{0:C}", _transfereeCustomer.Checking.Balance);

        rblToAccount.Items[1].Value = Convert.ToString(_transfereeCustomer.Saving.Balance);
        rblToAccount.Items[1].Text = "Saving - Balance: " + String.Format("{0:C}", _transfereeCustomer.Saving.Balance);

        CustomRangeValidator(rblToAccount);
    }

    protected void CustomRangeValidator(RadioButtonList rbl)
    {
        rqvAmountRange.Type = ValidationDataType.Double;
        rqvAmountRange.MinimumValue = "1";
        rqvAmountRange.MaximumValue = rbl.SelectedValue; 
        rqvAmountRange.SetFocusOnError = true;
        rqvAmountRange.Display = ValidatorDisplay.Dynamic;   
    }

    protected void calTransferDate_DayRender(object sender, DayRenderEventArgs e)
    {
        if (e.Day.Date < DateTime.Today)
        {
            e.Day.IsSelectable = false;
        }
    }

    protected void calTransferDate_SelectionChanged(object sender, EventArgs e)
    {
        txtTransferOn.Text = Convert.ToString(calTransferDate.SelectedDate.ToString("MM/dd/yyyy"));
    }

    protected void DisplayConfirmation()
    {

        var ddlFrom = (DropDownList) wizStep1.FindControl("ddlTransferor");
        var ddlTo = (DropDownList) wizStep3.FindControl("ddlTransferee");

        var transferor = CustomerDataAccess.getCustomerById(ddlFrom.SelectedValue);
        var transferee = CustomerDataAccess.getCustomerById(ddlTo.SelectedValue);

        var litConfirmNumber = new Literal
        {
            Text = "<h4>Confirmation Number: 040857354</h4>"
        };

        var litTransferor = new Literal
        {
            Text = "<h4>Transferor</h4>"
        };

        var lblTransferorName = new Label();
        var lblTransferorAccount = new Label();
        var lblTransferorAmount = new Label();

        lblTransferorName.Text = "Name: " + transferor;
        lblTransferorAccount.Text = "<br>Account: " + rblFromAccount.SelectedItem.Text;
        lblTransferorAmount.Text = "<br>Amount: " + txtAmount.Text;

        var litTransferee = new Literal
        {
            Text = "<h4>Transferee</h4>"
        };

        var lblTransfereeName = new Label();
        var lblTransfereeAccount = new Label();

        lblTransfereeName.Text = "Name: " + transferee;
        lblTransfereeAccount.Text = "<br>Account: " + rblToAccount.SelectedItem.Text;

        var litTransferDate = new Literal
        {
            Text = "<h4>Transfer Date: " + txtTransferOn.Text + "</h4>"
        };

        PlaceHolder display = null;
        PlaceHolder display1 = display;

        switch (wizFunds.ActiveStepIndex)
        {
            case 2:
                display = phReview;
                break;
            case 1:
                display = phReview;
                break;
            case 0:
                display = phReview;
                break;
            case 3:
                display = phConfirm;
                break;
        }

      //  if (display == null) return;

        phConfirm.Controls.Add(litConfirmNumber);
        display.Controls.Add(litTransferor);
        display.Controls.Add(lblTransferorName);
        display.Controls.Add(lblTransferorAccount);
        display.Controls.Add(lblTransferorAmount);
        display.Controls.Add(litTransferee);
        display.Controls.Add(lblTransfereeName);
        display.Controls.Add(lblTransfereeAccount);
        display.Controls.Add(litTransferDate);
    }

    protected void wizFunds_NextButtonClick(object sender, WizardNavigationEventArgs e)
    {
       
            DisplayConfirmation();
    }

    protected void wizFunds_FinishButtonClick(object sender, WizardNavigationEventArgs e)
    {
        frmWizard.Visible = false;
        pnlConfirm.Visible = true;
        DisplayConfirmation();
    }
}