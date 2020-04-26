using AndyTipsterPro.Entities;
using PayPal.v1.BillingPlans;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace AndyTipsterPro.Entities
{
    public class BillingPlan : BaseEntity
    {

        public string PayPalPlanId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string State { get; set; }
        public string PaymentFrequency { get; set; }
        public string PaymentInterval { get; set; }
        public string ReturnURL { get; set; }
        public string CancelURL { get; set; }
        public string CreateTime { get; set; }
        public string UpdateTime { get; set; }


        //[DataMember(Name = "create_time", EmitDefaultValue = false)]
        //public string CreateTime;
        //[DataMember(Name = "description", EmitDefaultValue = false)]
        //public string Description;
        //[DataMember(Name = "id", EmitDefaultValue = false)]
        //public string Id;
        //[DataMember(Name = "links", EmitDefaultValue = false)]
        //public List<LinkDescriptionObject> Links;
        //[DataMember(Name = "merchant_preferences", EmitDefaultValue = false)]
        //public MerchantPreferences MerchantPreferences;
        //[DataMember(Name = "name", EmitDefaultValue = false)]
        //public string Name;
        //[DataMember(Name = "payment_definitions", EmitDefaultValue = false)]
        //public List<PaymentDefinition> PaymentDefinitions;
        //[DataMember(Name = "state", EmitDefaultValue = false)]
        //public string State;
        //[DataMember(Name = "terms", EmitDefaultValue = false)]
        //public List<Terms> Terms;
        //[DataMember(Name = "type", EmitDefaultValue = false)]
        //public string Type;
        //[DataMember(Name = "update_time", EmitDefaultValue = false)]
        //public string UpdateTime;


    }
}
