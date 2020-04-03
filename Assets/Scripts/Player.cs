using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MovingObject
{
    public float restartLevelDelay = 1f;        
    public int hpPerFood = 10;               
    public int hpPerDrink = 20;                
    public int wallDamage = 1;                   


    private Animator animator;                 
    private int food;                           


    //Start overrides the Start function of MovingObject
    protected override void Start()
    {
        animator = GetComponent<Animator>();

        food = GameManager.instance.playerHP;

        base.Start();
    }


    private void OnDisable()
    {
        GameManager.instance.playerHP = food;
    }


    private void Update()
    {
        //Whose turn is it?
        if (!GameManager.instance.playersTurn) return;

        int horizontal = 0;      //Used to store the horizontal move input
        int vertical = 0;        //Used to store the vertical move input


        //Get input from the input manager, round it to an integer and store in horizontal to set x axis move input
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));

        //Get input from the input manager, round it to an integer and store in vertical to set y axis move input
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        //Check if moving horizontally, if so set vertical to zero.
        if (horizontal != 0)
        {
            vertical = 0;
        }

        //Check if we have a non-zero value for horizontal or vertical
        if (horizontal != 0 || vertical != 0)
        {
            AttemptMove<Wall>(horizontal, vertical);
        }
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        //Every time player moves, subtract from food points total.
        food--;

        //Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y input to move.
        base.AttemptMove<T>(xDir, yDir);

        //Hit allows us to reference the result of the Linecast done in Move.
        RaycastHit2D hit;

        //If Move returns true, meaning Player was able to move into an empty space.
        if (Move(xDir, yDir, out hit))
        {
            //Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
        }

        //Since the player has moved and lost food points, check if the game has ended.
        CheckIfGameOver();

        //Set the playersTurn boolean of GameManager to false now that players turn is over.
        GameManager.instance.playersTurn = false;
    }


    //This function handles obstruction events
    protected override void OnCantMove<T>(T component)
    {
        //Set the wall to be the one that the player is touching
        Wall hitWall = component as Wall;

        hitWall.DamageWall(wallDamage);

        //Trigger Player's attack animation
        animator.SetTrigger("playerChop");
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        //Check if the tag of the trigger collided with is Exit.
        if (other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);

            //Disable the player object since level is over.
            enabled = false;
        }

        //Check if the tag of the trigger collided with is Food.
        else if (other.tag == "Food")
        {
            //Add hpPerFood to the players current food total.
            food += hpPerFood;

            //Disable the food object the player collided with.
            other.gameObject.SetActive(false);
        }

        //Check if the tag of the trigger collided with is Soda.
        else if (other.tag == "Soda")
        {
            //Add hpPerDrink to players food points total
            food += hpPerDrink;


            //Disable the soda object the player collided with.
            other.gameObject.SetActive(false);
        }
    }


    //Restart reloads the scene when called.
    private void Restart()
    {
        //Load the last scene loaded, in this case Main, the only scene in the game.
        SceneManager.LoadScene(0);
    }


    //Called when a Player is attacked
    public void LoseFood(int loss)
    {
        //Set the trigger for the player animator to transition to the playerHit animation.
        animator.SetTrigger("playerHit");

        food -= loss;

        CheckIfGameOver();
    }


    //Is the player dead?
    private void CheckIfGameOver()
    {
        if (food <= 0)
        {
            GameManager.instance.GameOver();
        }
    }
}
