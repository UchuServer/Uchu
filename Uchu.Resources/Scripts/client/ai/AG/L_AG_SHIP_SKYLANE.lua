--this script automatically displays the complete mission window for the rocket parts
-- mision in the spaceship
function onMissionDialogueOK (self, msg)  

	if (msg.missionID == 308) then --check that it is the rocket parts mission
		
		 if (msg.iMissionState == 1) then --check to see if the player just accepted the mission
			
			--check to see if the player has any of the nosecone pie
			if ((msg.responder:GetInvItemCount{iObjTemplate = 4713}.itemCount == 1) or
				  (msg.responder:GetInvItemCount{iObjTemplate = 4716}.itemCount == 1) or
				  (msg.responder:GetInvItemCount{iObjTemplate = 4719}.itemCount == 1)) then
						 
				--check to see if the player has any cockpit pieces
                if ((msg.responder:GetInvItemCount{iObjTemplate = 4714}.itemCount == 1) or
                      (msg.responder:GetInvItemCount{iObjTemplate = 4717}.itemCount == 1) or
                      (msg.responder:GetInvItemCount{iObjTemplate = 4720}.itemCount == 1)) then
                        
					--check to see if the player has any engine pieces	 
                    if ((msg.responder:GetInvItemCount{iObjTemplate = 4715}.itemCount == 1) or
                      (msg.responder:GetInvItemCount{iObjTemplate = 4718}.itemCount == 1) or
                      (msg.responder:GetInvItemCount{iObjTemplate = 4721}.itemCount == 1)) then
					    --if the player has at least one of each of the rocket parts, start timer
						--the timer is necessary to make sure the current interaction with the skylane 
						--is done before it can start the next one
						GAMEOBJ:GetTimer():AddTimerWithCancel( 0.1, "completemission",self )   
			        end
					
                end
				
			end
			
		end
		
	end
	if (msg.missionID == 1896 and msg.iMissionState == 1) then --mission task header mission
		 UI:SendMessage( "DisplayTutorial", { {"type","mission"}, {"showImmediately", true} } )
	end
	
end

function onTimerDone(self,msg)
	if msg.name == "completemission" then
		--make the player interact with skylane again to complete the mission if they already have 
		--all the rocket parts when they get the mission
		local player = GAMEOBJ:GetObjectByID(GAMEOBJ:GetLocalCharID())
		player:SetPlayerInteraction{interaction = self}
		player:RequestUse{object = self }
		
	end
end
