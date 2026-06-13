namespace PharmaCO.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsPaidToOrders : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "IsPaid", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "IsPaid");
        }
    }
}
