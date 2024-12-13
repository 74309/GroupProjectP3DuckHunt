using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Transform[] spawnPoints;
    public GameObject duckPrefab;
    public SpriteRenderer bg;
    public Color blueColor;

    public GameObject dogMiss, dogHit;
    public SpriteRenderer dogSprite;
    public Sprite[] victorySprites;

    public TextMeshProUGUI roundText;
    public TextMeshProUGUI scoreText, hitsText, livesText;  // Added livesText

    int roundNumber = 1;
    int score, hits, ducksCreated;
    int lives = 3;  // Starting lives
    bool isRoundOver;
    bool isGameOver;  // Flag to track game over state
    Coroutine timeUpCoroutine;  // To hold reference to the TimeUp coroutine

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        UpdateUI();
        CallCreateDucks();
    }

    // Trigger duck creation
    public void CallCreateDucks()
    {
        if (isGameOver) return;  // Prevent new ducks from spawning if the game is over

        StartCoroutine(CreateDucks(2));  // Create 2 ducks per round
    }

    // Coroutine to handle duck creation
    IEnumerator CreateDucks(int _count)
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            yield break;  // Exit if no spawn points are assigned
        }

        yield return new WaitForSeconds(1);

        for (int i = 0; i < _count; i++)
        {
            if (isGameOver) yield break;  // Stop spawning ducks if the game is over

            int randomIndex = Random.Range(0, spawnPoints.Length);  // Pick a random spawn point
            GameObject _duck = Instantiate(duckPrefab, spawnPoints[randomIndex].position, Quaternion.identity);
            ducksCreated++;  // Increment the count of ducks created
        }

        // Start the countdown for the round time
        StartTimer();
    }

    // Start the round timer
    void StartTimer()
    {
        if (isGameOver) return;  // Don't start the timer if the game is over

        if (timeUpCoroutine != null)
        {
            StopCoroutine(timeUpCoroutine);  // Stop any active TimeUp coroutine
        }
        timeUpCoroutine = StartCoroutine(TimeUp());  // Start a new timer for the round
    }

    // Time countdown and actions once time's up
    IEnumerator TimeUp()
    {
        yield return new WaitForSeconds(10f);  // Wait for 10 seconds
        Duck[] ducks = FindObjectsOfType<Duck>();  // Find all ducks in the scene
        foreach (var duck in ducks)
        {
            duck.TimeUp();  // Notify ducks that the time is up
        }

        // Lose one life when time runs out
        LoseLife();

        bg.color = Color.red;  // Change background color to red
        yield return new WaitForSeconds(0.1f);
        bg.color = blueColor;  // Reset background color after the round ends

        if (!isRoundOver)
        {
            StartCoroutine(RoundOver());  // Start the round over sequence
        }
    }

    // Hit duck logic
    public void HitDuck()
    {
        if (isGameOver) return;  // Do nothing if the game is over
        StartCoroutine(Hit());
    }

    // Handle when a duck is hit
    IEnumerator Hit()
    {
        hits++;  // Increment hits count
        score += 10;  // Increase score by 10 per hit
        ducksCreated--;  // Decrease the ducks created count

        bg.color = Color.white;  // Flash the background
        yield return new WaitForSeconds(0.1f);
        bg.color = blueColor;  // Reset background color after flash

        // If all ducks are hit, or time is up, end the round
        if (ducksCreated <= 0)
        {
            if (!isRoundOver)
            {
                StopCoroutine(TimeUp());  // Stop the time-up countdown if all ducks are hit early
                StartCoroutine(RoundOver());
            }
        }

        UpdateUI();  // Update the UI after each hit
    }

    // Handle when a duck is missed (life lost)
    public void MissDuck()
    {
        if (isGameOver) return;  // Do nothing if the game is over
        LoseLife();  // Call the new method to handle life loss
        UpdateUI();
    }

    // Method to handle losing a life
    void LoseLife()
    {
        if (lives > 0)
        {
            lives--;  // Decrease lives
            if (lives == 0)
            {
                GameOver();  // End the game if no lives are left
            }
        }
    }

    // Handle the end of a round
    IEnumerator RoundOver()
    {
        isRoundOver = true;  // Mark round as over

        // Reset the background color to blue at the start of round over.
        bg.color = blueColor;

        yield return new WaitForSeconds(1f);  // Wait for a short time before showing results

        // Show appropriate dog sprite based on ducks left
        if (ducksCreated <= 0)
        {
            dogHit.SetActive(true);
            dogSprite.sprite = victorySprites[0];  // Full victory sprite
        }
        else if (ducksCreated == 1)
        {
            dogHit.SetActive(true);
            dogSprite.sprite = victorySprites[1];  // Partial victory sprite
        }
        else
        {
            dogMiss.SetActive(true);  // Miss sprite if ducks are left
        }

        // Wait a bit before transitioning to the next round
        yield return new WaitForSeconds(2f);
        dogHit.SetActive(false);
        dogMiss.SetActive(false);

        // Prepare for the next round
        roundNumber++;  // Increment the round number
        ducksCreated = 0;  // Reset the duck counter for the new round
        UpdateUI();  // Update the UI with the new round info

        CallCreateDucks();  // Start the next round and create ducks
        isRoundOver = false;  // Allow a new round to begin
    }

    // Game over logic
    private void GameOver()
    {
        isGameOver = true;  // Mark the game as over
        Debug.Log("Game Over!");
        // Stop any ongoing coroutines, such as creating ducks or handling rounds
        StopAllCoroutines();
        // Optionally, show the final score or transition to a different scene
    }

    // Update the UI elements
    private void UpdateUI()
    {
        roundText.text = "Round: " + roundNumber;  // Update round number
        scoreText.text = "Score: " + score;  // Update score
        hitsText.text = "Hits: " + hits;  // Update hits count
        livesText.text = "Lives: " + lives;  // Update lives count
    }

    // Check for a left mouse button click
    void Update()
    {
        if (isGameOver) return;  // Stop all inputs if the game is over

        if (Input.GetMouseButtonDown(0))  // 0 is the left mouse button
        {
            // Perform a raycast to check if a duck is clicked
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the hit object is a Duck
                if (hit.collider.gameObject.CompareTag("Duck"))
                {
                    // If the object is a duck, call HitDuck()
                    HitDuck();
                    return;  // Exit the function to prevent life loss
                }
            }

            // If no duck is hit, call MissDuck to remove a life
            MissDuck();
        }
    }
}

