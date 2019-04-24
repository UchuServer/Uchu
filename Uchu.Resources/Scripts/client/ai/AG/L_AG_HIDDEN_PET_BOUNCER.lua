--------------------------------------------------------------
-- Script on the bouncer for the pet dig to pet bouncer treasure node
-- this script makes sure the spawner network is cleared and reset after
-- each pet dig
-- the instructions for setting up a pet dig to pet boucner can be found
-- scripts/ai/AG/L_AG_PET_TREASURE_BOUNCER.lua
-- updated Brandi... 1/27/10
--------------------------------------------------------------

function onStartup(self,msg)

	GAMEOBJ:GetTimer():AddTimerWithCancel( 15, "BouncerTimer", self )

end

function onTimerDone(self, msg)

	if ( msg.name == "BouncerTimer" ) then
	
		local bouncerNumber = self:GetVar("BouncerNumber")
		
		if bouncerNumber then
		
			local petbouncer = LEVEL:GetSpawnerByName("PetBouncer"..bouncerNumber)
			local petbouncerswitch = LEVEL:GetSpawnerByName("PetBouncerSwitch"..bouncerNumber)
			
			petbouncer:SpawnerDestroyObjects()
			petbouncerswitch:SpawnerDestroyObjects()
			
		end
		
    end

end