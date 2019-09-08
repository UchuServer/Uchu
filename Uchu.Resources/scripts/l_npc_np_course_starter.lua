--------------------------------------------------------------
-- (CLIENT SIDE) Obstacle Course Starter NPC
--
-- Starts the course for the player
--------------------------------------------------------------

--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')
require('c_NimbusPark')
require('L_NP_NPC')

--------------------------------------------------------------
-- Startup of the object
--------------------------------------------------------------
function onStartup(self) 

    -- register ourself to be instructed later
    registerWithZoneControlObject(self)
    
    self:SetVar("IsPlayerInCourse", false)
    
    -- @TODO: change to leaderboard at somepoint
    self:SetVar("PlayerBestTime", 0)
    
	SetProximityDistance(self, 20)
    AddInteraction(self, "proximityText", Localize("NPC_NP_AMB_TALK_TO_ME_TO_START_THE_COURSE"))

end


--------------------------------------------------------------
-- Happens on client interaction
--------------------------------------------------------------
function onClientUse(self, msg)
    
    -- is player in course?
    if (self:GetVar("IsPlayerInCourse") == false) then
    
        local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
    
        strText = ""

        if (tonumber(self:GetVar("PlayerBestTime")) > 0) then
            strText = strText .. Localize("NPC_NP_AMB_YOUR_BEST_TIME") .. ": " .. ParseTime(self:GetVar("PlayerBestTime")) .. "\n\n"
        end

        strText = strText .. CONSTANTS["COURSE_STARTER_OFFER_TEXT"] .. "\n"
        

		-- show the summary message box
		player:DisplayMessageBox{bShow = true, 
								 imageID = 1, 
								 callbackClient = self, 
								 text = strText, 
								 identifier = "Course_Start"}
    else
        
        -- Actions for already being in course
        self:DisplayChatBubble{ wsText = CONSTANTS["COURSE_STARTER_ACTIVE_TEXT"] }
        
    end
    
end


--------------------------------------------------------------
-- Sent from a player when responding from a messagebox
--------------------------------------------------------------
function onMessageBoxRespond(self, msg)

    -- make sure this was from the local character
	if (GAMEOBJ:GetLocalCharID() == msg.sender:GetID()) then
	
	    -- user wants to start the course
	    if (msg.iButton == 1 and msg.identifier == "Course_Start") then
            -- tell the zone object about it	
            GAMEOBJ:GetZoneControlID():NotifyObject{ name="scene_4_course_start" }
            
            self:DisplayChatBubble{ wsText = CONSTANTS["COURSE_STARTER_ACTIVE_TEXT"] }
        end

	end

end



