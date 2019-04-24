
require('o_mis')


function onNotifyClientZoneObject(self,msg) 
 
  
 
	  if msg.name == "sendToAllclients_bubble" then


				for i = 1, 2 do

					for x = 1,  #self:MiniGameGetTeamPlayers{teamID = i}.objects do  
						local player = self:MiniGameGetTeamPlayers{teamID = i}.objects[x]
						local text = msg.paramStr
						--player:DisplayChatBubble{wsText = text}
					   -- print(text)
						--player:DisplayTooltip{ bShow = false, strText = text, iTime =  }
						 player:DisplayTooltip { bShow = true, strText = text, iTime = 3000 }

					end

				end

	  end
	  if msg.name == "unFreezAllPlayers" then
	   
	  		for i = 1, msg.param1 do
	  
	  			  for x = 1,  #self:MiniGameGetTeamPlayers{teamID = i}.objects do  
	  				  local player = self:MiniGameGetTeamPlayers{teamID = i}.objects[x]
	   				  player:SetUserCtrlCompPause{bPaused = false}
	  			  end
	  		end
	        
	    end
	    if msg.name == "FreezAllPlayers" then
	   
	  		for i = 1, msg.param1 do
	  
	  			  for x = 1,  #self:MiniGameGetTeamPlayers{teamID = i}.objects do  
	  				  local player = self:MiniGameGetTeamPlayers{teamID = i}.objects[x]
	   				  player:SetUserCtrlCompPause{bPaused = true}
	  			  end
	  		end
	        
  		end  
  		if msg.name == "sendToclient_bubble" then
		        local player = msg.paramObj
		        player:DisplayChatBubble{wsText = msg.paramStr}
  		end 
  
  
end 