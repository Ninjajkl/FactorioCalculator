namespace FactorioCalculator.Gleeba
{
    public static class FindMostEfficentCombo
    {
        private class Module
        {
            public string Name { get; set; }
            public int Tier { get; set; }
            public int Energy { get; set; }
            public int Speed { get; set; }
            public int Productivity { get; set; }

            public Module(string name, int tier, int e, int s, int p)
            {
                Name = name;
                Tier = tier;
                Energy = e;
                Speed = s;
                Productivity = p;
            }

            public override string ToString()
            {
                return Name;
            }
        }

        public static void Find()
        {
            List<Module> energyModules = [];
            List<Module> productivityModules = [];
            Console.Write("What's the max Tier: ");
            int.TryParse(Console.ReadLine(), out int eTier);
            switch (eTier)
            {
                //Lazy lol
                case 3:
                    energyModules.Add(new("E1", 1, -30, 0, 0));
                    productivityModules.Add(new("P1", 1, 40, -5, 4));
                    energyModules.Add(new("E2", 2, -40, 0, 0));
                    productivityModules.Add(new("P2", 2, 60, -10, 6));
                    energyModules.Add(new("E3", 3, -50, 0, 0));
                    productivityModules.Add(new("P3", 3, 80, -15, 10));
                    break;
                default:
                case 2:
                    energyModules.Add(new("E1", 1, -30, 0, 0));
                    productivityModules.Add(new("P1", 1, 40, -5, 4));
                    energyModules.Add(new("E2", 2, -40, 0, 0));
                    productivityModules.Add(new("P2", 2, 60, -10, 6));
                    break;
                case 1:
                    energyModules.Add(new("E1", 1, -30, 0, 0));
                    productivityModules.Add(new("P1", 1, 40, -5, 4));
                    break;
            }

            List<Module> AllModules = new(productivityModules)
            {
                energyModules.Last()
            };

            var result = new List<List<Module>>();

            var allResults = GenerateTwoGroupings(AllModules, 4);

            (List<Module> bestCombination, float BPS) = FindBestCombo(allResults);
            PrintBestCombo(bestCombination, BPS);
        }
        static List<List<Module>> GenerateTwoGroupings(List<Module> modules, int groupSize)
        {
            var allCombinations = new List<List<Module>>();

            // Generate valid combinations for one group of 4
            var combinations = new List<List<int>>();
            GenerateCombinations(modules.Count, groupSize, new List<int>(), combinations);

            // Create two groupings for each valid combination
            foreach (var firstGroupCounts in combinations)
            {
                foreach (var secondGroupCounts in combinations)
                {
                    var combinedGrouping = new List<Module>();

                    // Map the counts back to modules for the first group
                    for (int i = 0; i < modules.Count; i++)
                    {
                        combinedGrouping.AddRange(CreateModules(modules[i], firstGroupCounts[i]));
                    }

                    // Map the counts back to modules for the second group
                    for (int i = 0; i < modules.Count; i++)
                    {
                        combinedGrouping.AddRange(CreateModules(modules[i], secondGroupCounts[i]));
                    }

                    allCombinations.Add(combinedGrouping);
                }
            }

            return allCombinations;
        }

        static void GenerateCombinations(int moduleCount, int groupSize, List<int> current, List<List<int>> results)
        {
            if (current.Count == moduleCount)
            {
                if (Sum(current) == groupSize)
                    results.Add(new List<int>(current));
                return;
            }

            for (int i = 0; i <= groupSize - Sum(current); i++)
            {
                current.Add(i);
                GenerateCombinations(moduleCount, groupSize, current, results);
                current.RemoveAt(current.Count - 1);
            }
        }

        static List<Module> CreateModules(Module module, int count)
        {
            var modules = new List<Module>();
            for (int i = 0; i < count; i++)
            {
                modules.Add(module);
            }
            return modules;
        }

        static int Sum(List<int> list)
        {
            int total = 0;
            foreach (var num in list)
            {
                total += num;
            }
            return total;
        }

        static (List<Module> bestCombination, float BPS) FindBestCombo(List<List<Module>> ModuleCombos)
        {
            List<Module> bestCombination = [];
            float BPS = 0;
            foreach (var sequence in ModuleCombos)
            {
                //Console.WriteLine(string.Join(", ", sequence));
                List<Module> BioFluxMachine = sequence.GetRange(0, 4);

                int bfmEnergy = 100;
                int bfmProductivity = 150;
                int bfmSpeed = 200;

                foreach (var bfm in BioFluxMachine)
                {
                    bfmEnergy += bfm.Energy;
                    bfmProductivity += bfm.Productivity;
                    bfmSpeed += bfm.Speed;
                }
                bfmEnergy = Math.Max(20, bfmEnergy);

                //Console.WriteLine($"BioxFlux Energy: {bfmEnergy}%, Productivity: {bfmProductivity}%, Speed: {bfmSpeed}%");

                List<Module> NutrientMachine = sequence.GetRange(4, 4);

                int nutrientEnergy = 100;
                int nutrientProductivity = 150;
                int nutrientSpeed = 200;

                foreach (var nutrient in NutrientMachine)
                {
                    nutrientEnergy += nutrient.Energy;
                    nutrientProductivity += nutrient.Productivity;
                    nutrientSpeed += nutrient.Speed;
                }
                nutrientEnergy = Math.Max(20, nutrientEnergy);
                //Console.WriteLine($"Nutrient Energy: {nutrientEnergy}%, Productivity: {nutrientProductivity}%, Speed: {nutrientSpeed}%");

                float producedBioFluxPerBioFluxOperation = ((float)bfmProductivity / 100) * 4;
                float neededNutrientsPerBioFluxOperation = ((float)bfmEnergy / 100) * 0.75f;

                float producedNutrientsPerNutrientOperation = ((float)nutrientProductivity / 100) * 60;
                float neededBioFluxPerNutrientOperation = 5f;
                float neededNutrientsPerNutrientOperation = ((float)nutrientEnergy / 100) * 0.25f;

                float netBioFlux = producedBioFluxPerBioFluxOperation - (neededNutrientsPerBioFluxOperation / (producedNutrientsPerNutrientOperation - neededNutrientsPerNutrientOperation)) * neededBioFluxPerNutrientOperation;
                //Console.WriteLine($"Net Bio Flux: {netBioFlux} / operation");
                if (netBioFlux > BPS)
                {
                    BPS = netBioFlux;
                    bestCombination = new(sequence);
                }
            }
            return (bestCombination, BPS);
        }

        static void PrintBestCombo(List<Module> bestCombination, float BPS)
        {
            Console.WriteLine($"The best Combination is {string.Join(", ", bestCombination)}");

            List<Module> BioFluxMachine = bestCombination.GetRange(0, 4);

            int bfmEnergy = 100;
            int bfmProductivity = 150;
            int bfmSpeed = 200;

            foreach (var bfm in BioFluxMachine)
            {
                bfmEnergy += bfm.Energy;
                bfmProductivity += bfm.Productivity;
                bfmSpeed += bfm.Speed;
            }
            bfmEnergy = Math.Max(20, bfmEnergy);

            Console.WriteLine($"BioxFlux Energy: {bfmEnergy}%, Productivity: {bfmProductivity}%, Speed: {bfmSpeed}%");

            List<Module> NutrientMachine = bestCombination.GetRange(4, 4);

            int nutrientEnergy = 100;
            int nutrientProductivity = 150;
            int nutrientSpeed = 200;

            foreach (var nutrient in NutrientMachine)
            {
                nutrientEnergy += nutrient.Energy;
                nutrientProductivity += nutrient.Productivity;
                nutrientSpeed += nutrient.Speed;
            }
            nutrientEnergy = Math.Max(20, nutrientEnergy);
            Console.WriteLine($"Nutrient Energy: {nutrientEnergy}%, Productivity: {nutrientProductivity}%, Speed: {nutrientSpeed}%");

            float producedBioFluxPerBioFluxOperation = ((float)bfmProductivity / 100) * 4;
            Console.WriteLine($"Produced BioFlux per BioFlux Operation: {producedBioFluxPerBioFluxOperation}");

            float neededNutrientsPerBioFluxOperation = ((float)bfmEnergy / 100) * 0.75f;
            Console.WriteLine($"Needed Nutrients per BioFlux Operation: {neededNutrientsPerBioFluxOperation}");

            // Step 2: Calculate Nutrient creation parameters
            float producedNutrientsPerNutrientOperation = ((float)nutrientProductivity / 100) * 60;
            Console.WriteLine($"Produced Nutrients per Nutrient Operation: {producedNutrientsPerNutrientOperation}");

            float neededBioFluxPerNutrientOperation = 5f;
            Console.WriteLine($"Needed BioFlux per Nutrient Operation: {neededBioFluxPerNutrientOperation}");

            float neededNutrientsPerNutrientOperation = ((float)nutrientEnergy / 100) * 0.25f;
            Console.WriteLine($"Needed Nutrients per Nutrient Operation: {neededNutrientsPerNutrientOperation}");

            // Step 3: Calculate Nutrient Operations
            float nutrientOperations = neededNutrientsPerBioFluxOperation / (producedNutrientsPerNutrientOperation - neededNutrientsPerNutrientOperation);
            Console.WriteLine($"Nutrient Operations needed: {nutrientOperations}");

            // Step 4: Calculate BioFlux consumed for nutrient creation
            float bioFluxConsumedForNutrients = nutrientOperations * neededBioFluxPerNutrientOperation;
            Console.WriteLine($"BioFlux Consumed for Nutrients: {bioFluxConsumedForNutrients}");

            // Step 5: Calculate Net BioFlux
            float netBioFlux = producedBioFluxPerBioFluxOperation - bioFluxConsumedForNutrients;
            Console.WriteLine($"Net BioFlux: {netBioFlux}");

            Console.WriteLine($"With a Bioflux Per Operation of {BPS}");
        }
    }
}
