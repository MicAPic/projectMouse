EXTERNAL UnlockControls()
EXTERNAL UnlockAim()
EXTERNAL UnlockSelection()
EXTERNAL WaitForInput(inputType)
EXTERNAL EngendrarMiguel()
EXTERNAL ActivarMiguel()

->tutorial_p1

=== tutorial_p1 ===
Oh, hey there, Mousey.#auto: 
Since this is your first time playing, let me walk you over the basics.#auto: 
I'll unlock your controls right about... now.#auto: 
~ UnlockControls()
{WaitForInput("Move")}Alright, you can move freely. Use the <u>WASD</u> or the <u>Arrow keys</u>.
Nice, you're already getting used to it.#auto: 
See the wall of bullets on the right? No way you're passing that.#auto: 
Good thing you can dash right through. Just press <u>Space</u> or the <u>Right Mouse Button</u>.
* [get hurt]
    Well, that works too, I suppose.#auto: 
    ->tutorial_p2
* [don't get hurt]
    Oh, not bad, not bad at all.#auto: 
    ->tutorial_p2
    
=== tutorial_p2 ===
Just don't forget you can avoid bullets this way.#auto: 
And now we come to the real thing.#auto: 
I'll let my cousin Miguel help. He's your final challenge for today.#auto: 
~ EngendrarMiguel()
Defeat him in battle, and we will wrap it up.#auto: 
Oh, you're worried about fighting my cousin?#auto: 
Don't sweat it, that bird brain owes me a grand.#auto: 
~ UnlockAim()
Aim with your mouse and shoot by holding the <u>Left Mouse Button</u>. Go ahead! {ActivarMiguel()}
That will teach him a lesson or two!#auto:
Look! Somebody from your chat donated $100, and it was enough to meet the stream goal.#auto: 
Each time this happens, you can choose one of the three random power-ups.#auto: 
{UnlockSelection()}Navigate through them and use <u>Space</u> to make the choice.
Well done! I taught you everything I know.#auto: 
Now it's your time to shine. Show them what you're made of!#auto: 

->END