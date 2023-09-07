EXTERNAL UnlockControls()
EXTERNAL UnlockAim()
EXTERNAL UnlockSelection()
EXTERNAL WaitForInput(inputType)
EXTERNAL EngendrarMiguel()
EXTERNAL ActivarMiguel()
EXTERNAL ActivateTutorialTrigger()
EXTERNAL ToggleTip(state)

->tutorial_p1

=== tutorial_p1 ===
Oh, hello there, Mouse.#auto: 
Since this is your first time playing, let me walk you over the basics.#auto: 
{ToggleTip(true)}What, you thought I, Infernal Beelzebub Anarchia Cruul de Apokalypsi,#auto: 
the Opposite of Divine, Prince of Suffering and Torment, Master of all Sin,#auto: 
Conqueror of the Underworld, and King of all Demonkind,#auto: 
{ToggleTip(false)}could let you go into action right away? Don't make me laugh!#auto: 
Okay, I'll unlock your controls right about... now.#auto: 
~ UnlockControls()
{WaitForInput("Move")}Alright, you can move freely. Use the <u>WASD</u> or the <u>Arrow keys</u>.
Impressive, you're already getting used to it.#auto: 
See the wall of bullets on the right? No way you're passing that.#auto: 
{ActivateTutorialTrigger()}Good thing you can dash right through. Just press <u>Space</u> or the <u>Right Mouse Button</u>.
* [get hurt]
    Well, that works too, I suppose.#auto: 
    ->tutorial_p2
* [don't get hurt]
    I must admit, not bad at all.#auto: 
    ->tutorial_p2
    
=== tutorial_p2 ===
Just don't forget you can avoid bullets this way.#auto: 
And now we come to the real thing.#auto: 
I'll have one of my peasants, Miguel, help you. He's your final challenge for today.#auto: 
~ EngendrarMiguel()
Defeat him in battle, and we will wrap it up.#auto: 
Oh, you're worried about hurting my subordinate?#auto: 
Don't sweat it, that bird brain sold his soul to me a long time ago.#auto: 
~ UnlockAim()
Aim with your mouse and shoot by holding the <u>Left Mouse Button</u>. Go ahead! {ActivarMiguel()}
That will teach him a lesson or two!#auto:
Look! Somebody from your chat donated $100, and it was enough to meet the stream goal.#auto: 
Each time this happens, you can choose one of the three random power-ups.#auto: 
Navigate through them and use <u>Space</u> to make the choice.#auto: 
...{UnlockSelection()}
Glad you're putting my powers to good use.#auto: 
Can't believe I'm saying this, but... Show them what you're made of!#auto: 

->END