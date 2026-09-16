using System;

namespace FizzyLogicApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Medical Risk Assessment System");

    
            Console.Write("\nEnter Blood Glucose level (mg/dL): ");
            double bloodGlucose = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter Body Mass Index (BMI in kg/m²): ");
            double bmi = Convert.ToDouble(Console.ReadLine());

            //choose which model 
            Console.WriteLine("\nSelect Fuzzy Logic Model:");
            Console.WriteLine("1. Sugeno");
            Console.WriteLine("2. Mamdani");
            Console.Write("Enter choice (1 or 2): ");
            string choice = Console.ReadLine();

            //display of metrics
            Console.WriteLine("\n============================================");
            Console.WriteLine($"Patient Metrics:");
            Console.WriteLine($" - Blood Glucose: {bloodGlucose} mg/dL");
            Console.WriteLine($" - BMI:           {bmi} kg/m²");

            //fuzzification
            //glucose fuzzy sets: Normal, Prediabetic, Diabetic
            double gNormal = TrapezoidalMembership(bloodGlucose, 0, 0, 70, 100);
            double gPre = TriangularMembership(bloodGlucose, 90, 125, 160);
            double gDiab = TrapezoidalMembership(bloodGlucose, 140, 180, 300, 300);

            //bmi fuzzy sets: Normal, Overweight, Obese
            double bNormal = TrapezoidalMembership(bmi, 0, 0, 18.5, 24.9);
            double bOver = TriangularMembership(bmi, 23.0, 27.5, 32.0);
            double bObese = TrapezoidalMembership(bmi, 30.0, 35.0, 60.0, 60.0);

            Console.WriteLine("\n--- Fuzzification Values ---");
            Console.WriteLine($"Glucose - [Normal: {gNormal:F2}, Prediabetic: {gPre:F2}, Diabetic: {gDiab:F2}]");
            Console.WriteLine($"BMI     - [Normal: {bNormal:F2}, Overweight: {bOver:F2}, Obese: {bObese:F2}]");

            /*
             EVALUATION:
                - if glucose is normal and bmi is normal, risk is low
                - if glucose is prediabetic or bmi is overweight, risk is moderate  
                - if glucose is diabetic or glucose is prediabetic and bmi is obese, risk is high
             */
      
            double ruleLow = Math.Min(gNormal, bNormal);
            double ruleModerate = Math.Max(gPre, bOver);
            double ruleHigh = Math.Max(gDiab, Math.Min(gPre, bObese));

            double[] ruleStrengths = { ruleLow, ruleModerate, ruleHigh };

            Console.WriteLine("\n--- Rule Strengths ---");
            Console.WriteLine($"Rule 1 (Low Risk):      {ruleLow:F2}");
            Console.WriteLine($"Rule 2 (Moderate Risk): {ruleModerate:F2}");
            Console.WriteLine($"Rule 3 (High Risk):     {ruleHigh:F2}");

            //defuzzification
            double riskScore = 0.0;
            string selectedModel = "";

            if (choice == "2")
            {
                selectedModel = "Mamdani";
                riskScore = DefuzzifyMamdani(ruleStrengths, stepSize: 0.5);
            }
            else
            { 

                selectedModel = "Sugeno";
                riskScore = DefuzzifySugeno(ruleStrengths);
            }

            //summary & triage
            Console.WriteLine($"\n--- Diagnostic Summary ({selectedModel}) ---");
            Console.WriteLine($"Calculated Health Risk Index: {riskScore:F1}%");

            if (riskScore < 30.0)
                Console.WriteLine("Action Plan: Routine annual checkup.");
            else if (riskScore < 65.0)
                Console.WriteLine("Action Plan: Lifestyle modification & follow-up in 3 months.");
            else
                Console.WriteLine("Action Plan: Immediate medical intervention required.");

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        //mamdani method
        static double DefuzzifyMamdani(double[] ruleStrengths, double stepSize = 0.5)
        {
            double rLow = ruleStrengths[0];
            double rMod = ruleStrengths[1];
            double rHigh = ruleStrengths[2];

            double sumNumerator = 0.0;
            double sumDenominator = 0.0;

            for (double y = 0.0; y <= 100.0; y += stepSize)
            {
                //risk scale
                double outLow = TrapezoidalMembership(y, 0, 0, 15, 30);
                double outMod = TriangularMembership(y, 20, 50, 80);
                double outHigh = TrapezoidalMembership(y, 70, 85, 100, 100);

                //implications
                double clippedLow = Math.Min(rLow, outLow);
                double clippedMod = Math.Min(rMod, outMod);
                double clippedHigh = Math.Min(rHigh, outHigh);

                //aggregations
                double aggregatedY = Math.Max(clippedLow, Math.Max(clippedMod, clippedHigh));

                //centroid components
                sumNumerator += y * aggregatedY * stepSize;
                sumDenominator += aggregatedY * stepSize;
            }

            return sumDenominator > 0.0 ? (sumNumerator / sumDenominator) : 0.0;
        }

        //seguno method
        static double DefuzzifySugeno(double[] ruleStrengths)
        {
            double rLow = ruleStrengths[0];
            double rMod = ruleStrengths[1];
            double rHigh = ruleStrengths[2];

            double cLow = 15.0;
            double cMod = 50.0;
            double cHigh = 85.0;

            double numerator = (rLow * cLow) + (rMod * cMod) + (rHigh * cHigh);
            double denominator = rLow + rMod + rHigh;

            return denominator > 0.0 ? (numerator / denominator) : 0.0;
        }

        static double TriangularMembership(double x, double a, double b, double c)
        {
            if (x <= a || x >= c) return 0.0;
            if (x == b) return 1.0;
            if (x > a && x < b) return (x - a) / (b - a);
            return (c - x) / (c - b);
        }

        static double TrapezoidalMembership(double x, double a, double b, double c, double d)
        {
            if (x <= a || x >= d) return 0.0;
            if (x >= b && x <= c) return 1.0;
            if (x > a && x < b) return (x - a) / (b - a);
            return (d - x) / (d - c);
        }

    }
}
