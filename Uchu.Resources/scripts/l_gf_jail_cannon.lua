require('o_mis')

--[[function onGetPriorityPickListType(self, msg)
	local myPriority = 0.8
    if ( myPriority > msg.fCurrentPickTypePriority ) then
       msg.fCurrentPickTypePriority = myPriority
       msg.ePickType = 14    -- Interactive pick type
	end

    return msg

end ]]--


function onUse(self,msg)
-- set camera, lock player (teleport player?)
	local player = msg.user
	print(player)
	storeObjectByName(self, "Cannoneer", msg.user)
	player:PlayCinematic { pathName = "JailCam_"..self:GetVar('Jail')  }
	GAMEOBJ:GetTimer():AddTimerWithCancel( 3, "lightcannon",self )
	player:SetUserCtrlCompPause{bPaused = true}
-- play animation on player
	player:PlayAnimation{ animationID = "scratch" }
-- start timer for cannon animation to start



end

function onTimerDone(self,msg)
--if player animation done
-- play cannon firing animation
-- play cinematic of cannon ball flying

	local player = getObjectByName(self, "Cannoneer")
	
	if (msg.name == "lightcannon") then
		
		local cineTime = tonumber(LEVEL:GetCinematicInfo("JailCannonCam_"..self:GetVar('Jail')))
		player:PlayCinematic { pathName = "JailCannonCam_"..self:GetVar('Jail')  }
		GAMEOBJ:GetTimer():AddTimerWithCancel( 3, "explodedoor",self )
	
	end
	
	if (msg.name == "explodedoor") then
		player:PlayCinematic { pathName = "JailNinjaCam_"..self:GetVar('Jail')  }
		local ninja  = self:GetObjectsInGroup{group = "JailNinja"..self:GetVar('Jail'), ignoreSpawners = true}.objects[1]
		local jailcell = self:GetObjectsInGroup{group = "JailCell"..self:GetVar('Jail'), ignoreSpawners = true}.objects
		print("JailCell"..self:GetVar('Jail').. " " .. #jailcell) 
		for k,v in ipairs(jailcell) do    
			--GAMEOBJ:DeleteObject(v)
			print("v")
			v:Die{ killType = "VIOLENT"}
			
		end
	
		
		GAMEOBJ:GetTimer():AddTimerWithCancel( 3, "alldone",self )
		ninja:PlayAnimation{animationID = 'sleep', bPlayImmediate = true}
		
	end
	
	if (msg.name == "alldone") then
			print("alldone")
		player:SetUserCtrlCompPause{bPaused = false}
	end
			

end
