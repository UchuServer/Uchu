--------------------------------------------------------------
-- Description:
--
-- Hints for the player at Club LEGO form Max.
-- updated tgc... 6/04/10
--------------------------------------------------------------

require('L_AG_NPC')

function onStartup(self)
	                       
	--set the vars for interaction. NOTE: any/all of thses are optional
	SetProximityDistance(self, 30)

	
    -- Click on speech	
	AddInteraction(self, "interactionText", Localize("MAX_CLUBLEGO_CLICK_A")) --Click the arrow next to the circle at the top of the screen to see other missions you have.
	AddInteraction(self, "interactionText", Localize("MAX_CLUBLEGO_CLICK_B")) --Check your passport for achievements to do.  Doing achievements will get you all kinds of cool stuff!
	AddInteraction(self, "interactionText", Localize("MAX_CLUBLEGO_CLICK_C")) --You can always smash yourself in the Help Menu if you want.  It's fun!
	AddInteraction(self, "interactionText", Localize("MAX_CLUBLEGO_CLICK_D")) --If you can't collect stuff, check your backpack, it might be full.
	AddInteraction(self, "interactionText", Localize("MAX_CLUBLEGO_CLICK_E")) --You can fly to anywhere in LEGO Universe from here!
	AddInteraction(self, "interactionText", Localize("MAX_CLUBLEGO_CLICK_F")) --Have you tried racing yet?  Check out the race in Gnarled Forest, it's a lot of fun!
	AddInteraction(self, "interactionText", Localize("MAX_CLUBLEGO_CLICK_G")) --If you collect all five hidden bricks in Nimbus, you'll get a bigger backpack!
	AddInteraction(self, "interactionText", Localize("MAX_CLUBLEGO_CLICK_H")) --Look for flower stalks on the ground. Spray them with water and flowers will grow. Grow enough and you'll get a reward!
	AddInteraction(self, "interactionText", Localize("MAX_CLUBLEGO_CLICK_I")) --Look through all the binoculars you find, there might be a reward for you.
	AddInteraction(self, "interactionText", Localize("MAX_CLUBLEGO_CLICK_J")) --When in Nimbus Station, try playing all the instruments for thirty seconds and see what happens. Rock out!
	AddInteraction(self, "interactionText", Localize("MAX_CLUBLEGO_CLICK_K")) --Be sure and explore everywhere. You'll get cool stuff for finding all the hidden areas.
	AddInteraction(self, "interactionText", Localize("MAX_CLUBLEGO_CLICK_L")) --There is a Gorilla Mech in Gnarled Forest that is tough to smash. Build the bricks he pulls up from the ground into an anchor to beat him.
	AddInteraction(self, "interactionText", Localize("MAX_CLUBLEGO_CLICK_M")) --Smash all the enemies you come across.  There are added bonuses when you smash a lot of them.
	AddInteraction(self, "interactionText", Localize("MAX_CLUBLEGO_CLICK_N")) --Read all the message plates you come across to learn more about the LEGO Universe and you'll get something special for reading them all.
	AddInteraction(self, "interactionText", Localize("MAX_CLUBLEGO_CLICK_O")) --If you wear a Ninja cowl when using the rocket launcher to go to Forbidden Valley, you'll land near the tree instead of at the beginning.
	
	
	-- Proximity speech
	AddInteraction(self, "proximityText", Localize("MAX_CLUBLEGO_RANDOM_A"))  --Welcome Club LEGO Member!
	AddInteraction(self, "proximityText", Localize("MAX_CLUBLEGO_RANDOM_B"))  --This place is for Club LEGO members only, you're exclusive hang out spot.
	AddInteraction(self, "proximityText", Localize("MAX_CLUBLEGO_RANDOM_C"))  --Explore this place and have fun!
	AddInteraction(self, "proximityText", Localize("MAX_CLUBLEGO_RANDOM_D"))  --Being a Club member has tons of extra benifits.
	AddInteraction(self, "proximityText", Localize("MAX_CLUBLEGO_RANDOM_E"))  --Talk to me and I'll give you hints for the game.


end
