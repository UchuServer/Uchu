--------------------------------------------------------------
-- Includes
--------------------------------------------------------------
require('o_mis')

--------------------------------------------------------------
-- Constants
--------------------------------------------------------------
-- Start Location for the Zone
CONSTANTS = {}
--team A
CONSTANTS["TEAM_PLAYER_1"] = { objID = 0, name = 0, team = "A", score = 0, deaths = 0 }
CONSTANTS["TEAM_PLAYER_2"] = { objID = 0, name = 0, team = "A", score = 0, deaths = 0 }
CONSTANTS["TEAM_PLAYER_3"] = { objID = 0, name = 0, team = "A", score = 0, deaths = 0 }
CONSTANTS["TEAM_PLAYER_4"] = { objID = 0, name = 0, team = "A", score = 0, deaths = 0 }
--team B
CONSTANTS["TEAM_PLAYER_5"] = { objID = 0, name = 0, team = "B", score = 0, deaths = 0 }
CONSTANTS["TEAM_PLAYER_6"] = { objID = 0, name = 0, team = "B", score = 0, deaths = 0 }
CONSTANTS["TEAM_PLAYER_7"] = { objID = 0, name = 0, team = "B", score = 0, deaths = 0 }
CONSTANTS["TEAM_PLAYER_8"] = { objID = 0, name = 0, team = "B", score = 0, deaths = 0 }

CONSTANTS["PLAYER_1"] = "open"
CONSTANTS["PLAYER_2"] = "open"
CONSTANTS["PLAYER_3"] = "open"
CONSTANTS["PLAYER_4"] = "open"
CONSTANTS["PLAYER_5"] = "open"
CONSTANTS["PLAYER_6"] = "open"
CONSTANTS["PLAYER_7"] = "open"
CONSTANTS["PLAYER_8"] = "open"


function onNotifyClientZoneObject(self,msg)
    
    -- [ score ] ---------------   20
    
    
    -- [ deaths ] ---------------- 15
    
    
    --[ objID ] ----------------   10
	-- param1 = 10 ADD PLAYER ID 
	    -- name -- objID
	    -- param2  TEAM 1 = A 2 = B

	if msg.param1 == 10 then
	
		--add player string.name and update team
		-- param1 == ObjID(int) ,   param2 = (1 = team A) (2 = team B)
		--if msg.param2 == 1 then
		
			for i = 1, 8 do
				if CONSTANTS["TEAM_PLAYER_"..i].objID == 0 then
                    CONSTANTS["TEAM_PLAYER_"..i].name =   msg.name
				  	print("SAVED TO CLIENT ="..tostring(CONSTANTS["TEAM_PLAYER_"..i].name))
				  	if msg.param2 == 1 then
				  	    self:SetVar(tostring(CONSTANTS["TEAM_PLAYER_"..i].name), "A") 
				  	else
				  	    self:SetVar(tostring(CONSTANTS["TEAM_PLAYER_"..i].name), "B") 
				  	end
				   break
				end
			
			end
		

		
		
		--end
	
	end



end
function onPlayerLoaded(self, msg)
    local player = msg.playerID
    
    for i = 1, 8 do
        if CONSTANTS["PLAYER_"..i] == "open" then
            CONSTANTS["PLAYER_"..i] = player
        end
    
    end
end


function GetTeam(self, name )
    for i = 1, 8 do
        if name == CONSTANTS["TEAM_PLAYER_"..i].name then
            return CONSTANTS["TEAM_PLAYER_"..i].team
        end
    end

end
