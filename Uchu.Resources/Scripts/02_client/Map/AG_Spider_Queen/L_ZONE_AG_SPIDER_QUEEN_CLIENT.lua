----------------------------------------------------------------
-- level specific client script for Property Pushback in AG small property
-- this script requires a base script
-- this script should be in the zone script in the DB
--
-- updated abeechler ... 7/14/11 - now extends from AG Property script
----------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('02_client/Map/Property/AG_Small/L_ZONE_AG_PROPERTY_CLIENT')
        
--//////////////////////////////////////////////////////////////////////////////////
-- User Config local variables

-- player flags. These have to be different for each property map. these are set up in the db
local flags = {
				defeatedPropFlag = -1 -- when the player builds the claimmarker defeating the maelstrom on this property
			  }
--GROUPS, set in Happy Flower on objects, make sure these match the server script
local Groups = {
				PlaqueGroup = "PropertyPlaque",
				Guard = "Guard"
			   }
			   
----------------------------------------------------------------
-- Store music var references
----------------------------------------------------------------
local MaelMusicCue = "AG_Survival_2"
local ClearedMusicCue = "Property_Peaceful"

----------------------------------------------------------------
-- Startup, Sets up us some variables
----------------------------------------------------------------
function onStartup(self)
    setGameVariables(Groups,flags,ClearedMusicCue,MaelMusicCue)
    self:SetVar("GUIDPeaceful3D", GUIDPeaceful3D)
end
        
----------------------------------------------------------------
-- leave the functions below alone
----------------------------------------------------------------

function onServerDoneLoadingAllObjects(self, msg)
    UI:SendMessage( "ToggleActivityCloseButton", {  {"bShow", true}, 
                                            {"GameObject", self}, 
                                            {"MessageName", "toLua"}, 
                                            {"senderID", player} } )
end

function ExitBox(self, player)
    local player = GAMEOBJ:GetControlledID() 	
    
    if player:Exists() then
		local text = Localize("SURVIVAL_EXIT_QUESTION")
		
		if self:GetNetworkVar("wavesStarted") then
			text = Localize("SURVIVAL_PLAYING_EXIT_QUESTION") -- "Your time will not be recorded if you exit before the game is over. Exit anyway?"
		end
		
        -- display exit box
        player:DisplayMessageBox{bShow = true, 
                         imageID = 1, 
                         text = text, 
                         callbackClient = self, 
                         identifier = "Exit"}
    end
end

----------------------------------------------------------------
-- Sent from a player when responding from a messagebox
----------------------------------------------------------------
function onMessageBoxRespond(self, msg)        
    if msg.identifier == "ActivityCloseButtonPressed" and msg.iButton == -1 then  
        -- when the player hits the activity close button, the x in the top right
        ExitBox(self, player)
    end
end
