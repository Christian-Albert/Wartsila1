namespace Wartsila1
{
    // Gas class for name and data values of a certain gas
    public class Gas
    {
        public string Name
        { 
            get; 
            set; 
        }

        public float Enthalpy
        {
            get; 
            set; 
        }

        public float Fraction
        {
            get;
            set;
        }

        // We need this override to represent the Gas object as a string
        public override string ToString()
        {
            return this.Name;
        }

    }    
}
