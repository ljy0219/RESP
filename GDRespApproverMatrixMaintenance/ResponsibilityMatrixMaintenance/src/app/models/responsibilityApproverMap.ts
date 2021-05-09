export class ResponsibilityApproverMap {

    public ID: number;
    public Instance: string;
    public Ap_Group: string;
    public Division: string;
    public Responsibility_Name: string;
    public Application: string;
    public Primary_Approver: string;
    public Secondary_Approver: string;
    public Final_Approver: string;
    public Comment: string;
    public Sod_Active: string;
    public Last_Updated_By: string;
    public Last_Updated_Date: Date;
    public Approvers_Not_Listed: string;
    public Default: string;
    public PriorPrimaryApprover: string;
    public PriorPrimaryApproverUpdatedDate: Date;
    public PriorSecondaryApprover: string;
    public PriorSecondaryApproverUpdatedDate: Date;
    public PriorFinalApprover: string;
    public PriorFinalApproverUpdatedDate: Date;

    // read only memebers
    public Available_in_Production: string;
    public Active: string;
    public Start_Date: Date;
    public End_Date: Date;
    public Do_Not_Use: string;

}
