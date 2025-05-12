using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Linq; // Needed for .SequenceEqual() and .ToList()

// Structure to hold a cocktail recipe
[System.Serializable]
public struct CocktailRecipe
{
    public string drinkName; // e.g., "Negroni"
    public List<string> ingredients; // e.g., {"Gin", "Campari", "Sweet Vermouth"}
    public string preparationMethod; // "Stir" or "Shake"
    public Sprite finishedDrinkSprite;
}

public class GameManager : MonoBehaviour
{
    // --- Guest and Spawn Variables ---
    public GameObject guestPrefab;
    public Transform[] guestSpots;
    public float spawnInterval = 5f;
    private float timeSinceLastSpawn;
    private List<string> availableDrinks; // Populated from recipes

    // --- Recipe Variables ---
    public List<CocktailRecipe> recipes; // List of all recipes

    // --- Preparation Variables ---
    private ClickableItem currentContainer = null; // The currently selected glass/shaker
    private List<string> ingredientsInContainer = new List<string>(); // Ingredients added to the container

    // --- UI Variables ---
    public TextMeshProUGUI preparationStatusText; // UI Text for preparation status

    // --- Finished Drink Variables ---
    public GameObject finishedDrinkPrefab; // Prefab for the finished drink item
    public Transform drinkSpawnSpot; // Location on the counter where finished drinks appear

    // --- Serving Variables ---
    private ClickableItem heldDrink = null; // The finished drink the player is holding to serve

    void Start()
    {
        timeSinceLastSpawn = spawnInterval;
        if (preparationStatusText != null)
        {
            preparationStatusText.text = "Выберите стакан или шейкер";
        }

        // Initialize Recipes (Can also be done directly in the Inspector)
        // If you are filling recipes in the Inspector, ensure this block is removed
        // or only runs if the recipes list is empty.
        if (recipes == null || recipes.Count == 0)
        {
            recipes = new List<CocktailRecipe>();
            // Negroni Recipe: Gin, Campari, Sweet Vermouth, Stir
            recipes.Add(new CocktailRecipe
            {
                drinkName = "Negroni",
                ingredients = new List<string> { "Gin", "Campari", "Vermouth" },
                preparationMethod = "Stir" // Stirred, typically in a Mixing Glass
            });
            // Gimlet Recipe: Gin, Lime Juice, Simple Syrup, Shake
            recipes.Add(new CocktailRecipe
            {
                drinkName = "Gimlet",
                ingredients = new List<string> { "Gin", "LimeJuice", "Simple" },
                preparationMethod = "Shake" // Shaken, in a Shaker
            });
            // Add other recipes here...
        }

        // Populate available drinks list from recipes
        availableDrinks = recipes.Select(r => r.drinkName).ToList();

        // Ensure guest spots are assigned if not using inspector
        if (guestSpots == null || guestSpots.Length == 0)
        {
            // Find guest spots by tag or name pattern if not assigned manually
            // Example: guestSpots = GameObject.FindGameObjectsWithTag("GuestSpot").Select(go => go.transform).ToArray();
            Debug.LogWarning("Guest spots are not assigned in GameManager inspector!");
        }

        // Ensure finished drink prefab and spawn spot are assigned
        if (finishedDrinkPrefab == null) Debug.LogWarning("Finished Drink Prefab is not assigned in GameManager inspector!");
        if (drinkSpawnSpot == null) Debug.LogWarning("Drink Spawn Spot is not assigned in GameManager inspector!");
    }

    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= spawnInterval)
        {
            TrySpawnGuest();
            timeSinceLastSpawn = 0f;
        }
    }

    // Method to handle clicks on objects with ClickableItem
    public void HandleItemClick(ClickableItem item)
    {
        Debug.Log("GameManager received click: " + item.itemName + " (" + item.itemType + ")");

        if (heldDrink != null) // If the player is holding a finished drink
        {
            if (item.itemType == "Guest")
            {
                // Attempt to serve the held drink to the clicked guest
                ServeDrinkToGuest(heldDrink, item.GetComponent<Guest>());
                // After serving (successful or not), clear the held drink
                heldDrink = null;
                // TODO: Visually remove or return the held drink sprite that might follow the cursor
            }
            else
            {
                // Player clicked something else while holding a drink.
                Debug.Log("Serving of drink " + heldDrink.itemName + " cancelled.");
                // Option 1: Destroy the held drink
                // Destroy(heldDrink.gameObject);
                // Option 2: Return the held drink to the spawn spot
                // if (drinkSpawnSpot != null) heldDrink.transform.position = drinkSpawnSpot.position;
                // Option 3: Simply clear the held drink state (assuming it was a visual representation)
                heldDrink = null; // Just clear the state for now
                if (preparationStatusText != null)
                {
                    preparationStatusText.text = "Serving cancelled. " + (currentContainer == null ? "Select a glass or shaker" : "Preparing in: " + currentContainer.itemName);
                }
            }
        }
        else // Player is not holding a drink, clicking for preparation
        {
            if (item.itemType == "Tool")
            {
                // Clicked on a tool (MixingGlass, Shaker)
                if (item.itemName == "MixingGlass" || item.itemName == "Shaker")
                {
                    if (item == currentContainer && ingredientsInContainer.Count > 0) // Only finish if ingredients were added
                    {
                        FinishPreparation(); // Finish the preparation process (Stir/Shake happens now)
                    }
                    else if (item == currentContainer && ingredientsInContainer.Count == 0)
                    {
                        // Clicked on the selected container but no ingredients added
                        Debug.Log("Container selected, add ingredients.");
                        if (preparationStatusText != null)
                        {
                            preparationStatusText.text = "Preparing in: " + currentContainer.itemName + ". Add ingredients.";
                        }
                    }
                    else
                    {
                        StartPreparation(item);
                    }
                }
                // TODO: Add logic for other tools if needed
            }
            else if (item.itemType == "Ingredient")
            {
                // Clicked on an ingredient
                if (currentContainer == null) // Check if a container is selected first
                {
                    Debug.Log("Select a glass or shaker first!");
                    if (preparationStatusText != null)
                    {
                        preparationStatusText.text = "Select a glass or shaker first";
                    }
                    return; // Stop here if no container is selected
                }
                AddIngredientToContainer(item);
            }
            else if (item.itemType == "Drink")
            {
                // Clicked on a finished drink object to pick it up
                heldDrink = item;
                Debug.Log("Player picked up drink: " + item.itemName + ". Click on a guest to serve.");
                if (preparationStatusText != null)
                {
                    preparationStatusText.text = "Drink \"" + item.itemName + "\" ready. Click on a guest.";
                }
                // TODO: Visually 'attach' the heldDrink sprite to the cursor position.
            }
        }
    }

    // Starts the preparation process with a selected container (MixingGlass or Shaker)
    void StartPreparation(ClickableItem container)
    {
        // Reset any previous preparation state
        ResetPreparation();

        currentContainer = container;
        ingredientsInContainer.Clear(); // Clear the list of ingredients for the new preparation

        if (preparationStatusText != null)
        {
            preparationStatusText.text = "Preparing in: " + container.itemName + ". Add ingredients.";
        }
        Debug.Log("Preparation started in: " + container.itemName);

        // TODO: Add visual feedback for the selected container (e.g., outline, highlight)
    }

    // Adds an ingredient to the currently selected container
    void AddIngredientToContainer(ClickableItem ingredient)
    {
        // This check is now done in HandleItemClick before calling this method
        // if (currentContainer == null) { ... }

        // Add the ingredient name to the list
        ingredientsInContainer.Add(ingredient.itemName);

        // TODO: Implement visual representation of ingredients inside the container
        // e.g., change container sprite, add small ingredient sprites, update text dynamically

        if (preparationStatusText != null)
        {
            // Append the ingredient name to the status text
            preparationStatusText.text += "\n- " + ingredient.itemName;
        }
        Debug.Log("Added ingredient: " + ingredient.itemName + " to " + currentContainer.itemName);
    }

    // Attempts to finalize the preparation (perform Stir or Shake) and create the finished drink
    public void FinishPreparation()
    {
        if (currentContainer == null || ingredientsInContainer.Count == 0)
        {
            Debug.Log("Nothing to finish. Select a container and add ingredients.");
            if (preparationStatusText != null)
            {
                preparationStatusText.text = "Nothing to finish";
            }
            return;
        }

        Debug.Log("Attempting to finish preparation...");
        // Attempt to determine the resulting drink based on ingredients and method
        string resultingDrinkName = DetermineDrink();

        if (!string.IsNullOrEmpty(resultingDrinkName))
        {
            Debug.Log("Successfully prepared: " + resultingDrinkName);
            if (preparationStatusText != null)
            {
                preparationStatusText.text = "Prepared: " + resultingDrinkName + ". Serve the guest!";
            }
            // Create the finished drink item
            CreateFinishedDrinkObject(resultingDrinkName);
            // Reset preparation state after successfully creating the finished drink object
            // ResetPreparation(); // Moved inside CreateFinishedDrinkObject or call after successful creation
        }
        else
        {
            Debug.Log("Result is not a known cocktail or wrong method used. Resetting preparation.");
            if (preparationStatusText != null)
            {
                preparationStatusText.text = "Not a known cocktail or wrong method used.";
            }
            // If the result is not a known drink, reset the preparation
            ResetPreparation();
            // TODO: Optionally create a "failed drink" object or visual/sound feedback for failure
        }
    }
    public bool IsCocktailComplete { get; set; }

    public void CompleteCocktail()
    {
        IsCocktailComplete = true;
        FindObjectOfType<Shaker>().OnCocktailComplete();
    }

    // Determines which cocktail recipe matches the ingredients and preparation method
    string DetermineDrink()
    {
        Debug.Log("--- Starting DetermineDrink ---");

        // If no ingredients, it's definitely not a cocktail
        if (ingredientsInContainer.Count == 0)
        {
            Debug.Log("DetermineDrink: No ingredients in container.");
            Debug.Log("--- Ending DetermineDrink (No Ingredients) ---");
            return null;
        }

        // If no container selected, method is undefined
        if (currentContainer == null)
        {
            Debug.Log("DetermineDrink: No container selected.");
            Debug.Log("--- Ending DetermineDrink (No Container) ---");
            return null;
        }

        Debug.Log($"DetermineDrink: Checking for a match with {ingredientsInContainer.Count} ingredients in {currentContainer.itemName}.");

        // Sort the current ingredients for order-independent comparison
        List<string> currentIngredientsSorted = new List<string>(ingredientsInContainer);
        currentIngredientsSorted.Sort();

        Debug.Log($"DetermineDrink: Current ingredients (sorted): [{string.Join(", ", currentIngredientsSorted)}]");
        Debug.Log($"DetermineDrink: Used container: {currentContainer.itemName}");


        foreach (var recipe in recipes)
        {
            Debug.Log($"--- Checking recipe: {recipe.drinkName} ---");
            Debug.Log($"Recipe Method: {recipe.preparationMethod}, Recipe Ingredients: [{string.Join(", ", recipe.ingredients)}]");

            // Sort the recipe ingredients for comparison
            List<string> recipeIngredientsSorted = new List<string>(recipe.ingredients);
            recipeIngredientsSorted.Sort();

            Debug.Log($"Recipe ingredients (sorted): [{string.Join(", ", recipeIngredientsSorted)}]");

            // 1. Check if ingredient count matches
            if (recipeIngredientsSorted.Count != currentIngredientsSorted.Count)
            {
                Debug.Log($"DetermineDrink: Ingredient count mismatch for {recipe.drinkName}. Expected {recipeIngredientsSorted.Count}, got {currentIngredientsSorted.Count}.");
                continue; // Move to the next recipe
            }
            Debug.Log($"DetermineDrink: Ingredient counts match for {recipe.drinkName}.");


            // 2. Check if the sorted ingredient lists are identical
            bool ingredientsMatch = recipeIngredientsSorted.SequenceEqual(currentIngredientsSorted);
            if (!ingredientsMatch)
            {
                Debug.Log($"DetermineDrink: Ingredients list does not match for {recipe.drinkName}.");
                continue; // Move to the next recipe
            }
            Debug.Log($"DetermineDrink: Ingredients match for {recipe.drinkName}!");


            // 3. Check if the preparation method matches the used container
            bool methodMatches = false;
            string requiredContainerForMethod = "None specified"; // For debugging message

            if (recipe.preparationMethod == "Stir")
            {
                requiredContainerForMethod = "MixingGlass";
                if (currentContainer.itemName == "MixingGlass")
                {
                    methodMatches = true;
                }
            }
            else if (recipe.preparationMethod == "Shake")
            {
                requiredContainerForMethod = "Shaker";
                if (currentContainer.itemName == "Shaker")
                {
                    methodMatches = true;
                }
            }
            // Add checks for other methods/containers if needed in the future
            // For Negroni and Gimlet, Stir needs MixingGlass, Shake needs Shaker.

            if (!methodMatches)
            {
                Debug.Log($"DetermineDrink: Method/Container mismatch for {recipe.drinkName}. Recipe requires method '{recipe.preparationMethod}' (container '{requiredContainerForMethod}'), but used container is '{currentContainer.itemName}'.");
                continue; // Move to the next recipe
            }
            Debug.Log($"DetermineDrink: Method/Container matches for {recipe.drinkName}!");


            // If both ingredients and method match, this is the drink!
            Debug.Log($"--- Found matching recipe: {recipe.drinkName} ---");
            Debug.Log("--- Ending DetermineDrink (Match Found) ---");
            return recipe.drinkName;
        }

        // No matching recipe found after checking all
        Debug.Log("--- No matching recipe found for the current ingredients and method ---");
        Debug.Log("--- Ending DetermineDrink (No Match) ---");
        return null;
    }

    // Creates an instance of the finished drink prefab
    void CreateFinishedDrinkObject(string drinkName)
    {
        if (finishedDrinkPrefab != null && drinkSpawnSpot != null)
        {
            // Find the recipe that matches the determined drink name
            CocktailRecipe matchedRecipe = recipes.Find(r => r.drinkName == drinkName);

            if (matchedRecipe.drinkName != null && matchedRecipe.finishedDrinkSprite != null) // Check if recipe and sprite are found
            {
                GameObject finishedDrinkObj = Instantiate(finishedDrinkPrefab, drinkSpawnSpot.position, Quaternion.identity);
                ClickableItem finishedDrinkItem = finishedDrinkObj.GetComponent<ClickableItem>();
                SpriteRenderer spriteRenderer = finishedDrinkObj.GetComponent<SpriteRenderer>(); // Get the SpriteRenderer

                if (finishedDrinkItem != null && spriteRenderer != null)
                {
                    finishedDrinkItem.itemName = drinkName; // Set the drink name
                    finishedDrinkItem.itemType = "Drink"; // Set the type
                    spriteRenderer.sprite = matchedRecipe.finishedDrinkSprite; // <-- НАЗНАЧАЕМ СПРАЙТ

                    Debug.Log("Created finished drink object: " + drinkName);

                    // Display status
                    if (preparationStatusText != null)
                    {
                        preparationStatusText.text = "Ready: " + drinkName + ". Serve it!";
                    }

                    // After creating the finished drink object, reset the preparation state
                    ResetPreparation();
                }
                else
                {
                    if (finishedDrinkItem == null) Debug.LogError("Finished drink prefab is missing the ClickableItem component!");
                    if (spriteRenderer == null) Debug.LogError("Finished drink prefab is missing the SpriteRenderer component!");
                    ResetPreparation(); // Reset even if components are missing
                }
            }
            else
            {
                if (matchedRecipe.drinkName == null) Debug.LogError($"Recipe for drink name '{drinkName}' not found!");
                if (matchedRecipe.finishedDrinkSprite == null) Debug.LogError($"Sprite for drink '{drinkName}' is not assigned in the recipe!");
                ResetPreparation(); // Reset if recipe or sprite is missing
            }
        }
        else
        {
            Debug.LogError("Finished Drink Prefab or Drink Spawn Spot is not assigned!");
            ResetPreparation();
        }
    }

    // Resets the preparation state
    void ResetPreparation()
    {
        currentContainer = null;
        ingredientsInContainer.Clear();
        if (preparationStatusText != null)
        {
            preparationStatusText.text = "Выберите стакан или шейкер";
        }
        Debug.Log("Preparation reset.");
        // TODO: Reset visual state of containers (e.g., clear internal sprites)
    }

    // Gets the Guest component at a specific guest spot transform
    public Guest GetGuestAtSpot(Transform spot)
    {
        if (spot.childCount > 0)
        {
            return spot.GetChild(0).GetComponent<Guest>();
        }
        return null;
    }

    // Serves a finished drink to a target guest
    public void ServeDrinkToGuest(ClickableItem finishedDrink, Guest targetGuest)
    {
        if (finishedDrink == null || targetGuest == null)
        {
            Debug.Log("Nothing or no one to serve.");
            if (preparationStatusText != null)
            {
                preparationStatusText.text = "Nothing or no one to serve.";
            }
            return;
        }

        Debug.Log($"Attempting to serve {finishedDrink.itemName} to guest who ordered {targetGuest.orderedDrink}");

        bool correctDrink = targetGuest.ServeDrink(finishedDrink.itemName); // Let the guest check the drink

        if (correctDrink)
        {
            Debug.Log("Guest is happy! +Score!");
            // TODO: Add scoring logic
            targetGuest.Leave(); // Guest leaves if satisfied
        }
        else
        {
            Debug.Log("Guest is unhappy! -Score/No Score!");
            // TODO: Add negative scoring or no score logic
            targetGuest.Leave(); // Guest leaves even if unhappy (maybe with different animation)
        }

        // Destroy the served drink object
        Destroy(finishedDrink.gameObject); // This removes the object that was clicked/held

        // Reset status text
        if (preparationStatusText != null)
        {
            preparationStatusText.text = "Select a glass or shaker"; // Or a different starting status
        }
        heldDrink = null; // Ensure heldDrink state is cleared
    }

    // --- Guest Spawn Logic (from previous step, ensure it's included) ---
    void TrySpawnGuest()
    {
        // Find a free spot for a guest
        Transform freeSpot = null;
        foreach (Transform spot in guestSpots)
        {
            // Check if the spot is occupied (by checking for children)
            if (spot.childCount == 0)
            {
                freeSpot = spot;
                break;
            }
        }

        if (freeSpot != null)
        {
            // Create a new guest from the prefab at the free spot's position
            GameObject newGuestObj = Instantiate(guestPrefab, freeSpot.position, Quaternion.identity);
            newGuestObj.transform.SetParent(freeSpot); // Make the guest a child of the spot

            Guest newGuest = newGuestObj.GetComponent<Guest>();
            if (newGuest == null)
            {
                Debug.LogError("Guest prefab is missing the Guest script!");
                Destroy(newGuestObj); // Clean up if prefab is misconfigured
                return;
            }

            // !!! Important: Select a drink only from the available recipes !!!
            if (availableDrinks != null && availableDrinks.Count > 0)
            {
                string randomDrink = availableDrinks[Random.Range(0, availableDrinks.Count)];
                newGuest.SetOrder(randomDrink);

                // Ensure the guest prefab has a ClickableItem component with itemType="Guest"
                ClickableItem guestClickItem = newGuestObj.GetComponent<ClickableItem>();
                if (guestClickItem == null)
                {
                    guestClickItem = newGuestObj.AddComponent<ClickableItem>();
                }
                guestClickItem.itemType = "Guest";
                guestClickItem.itemName = "Guest"; // Can be unique if needed

                Debug.Log("New guest arrived and ordered: " + randomDrink);
            }
            else
            {
                Debug.LogError("No available drinks in recipes for guest spawn!");
                Destroy(newGuestObj); // Destroy the guest if no drinks can be ordered
            }
        }
        else
        {
            Debug.Log("No free spots for guests.");
        }
    }
}
