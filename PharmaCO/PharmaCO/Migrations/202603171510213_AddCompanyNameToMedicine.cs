namespace PharmaCO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCompanyNameToMedicine : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Medicines", "CompanyName", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Medicines", "CompanyName");
        }
    }
}
