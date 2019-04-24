require('o_mis')


function onStartup(self)

	Con = {}
	for i = 1, 4 do 

		Con["inActivity_"..i] = nil

	end

	self:SetVar("Con",Con)


end

function onCollisionPhantom(self, msg)
		
	local target = msg.objectID
	local faction = target:GetFaction()
	
	if faction and faction.faction == 1 then
		
		
		 ActivityAdd(self, target)
		
	end   
        
       
end



function ActivityAdd(self, player)

	--------------------------------------
	--   Store Activity Object if nil   --
	--------------------------------------
	if not self:GetVar("ActivityObj") then
	
	 	local ActivityObj = self:GetObjectsInGroup{ group = "ActivityObj" ,ignoreSpawners = true }.objects
	 	ActivityObj[1]:NotifyObject{name = "storeIntroTrigger", ObjIDSender = self }	
	 	storeObjectByName(self, "ActivityObj", ActivityObj[1])
	 	
	end

	local ActivityObj = getObjectByName(self, "ActivityObj")
	
	
	if not ActivityObj:ActivityUserExists{userID = player}.bExists then
	
		ActivityObj:NotifyObject{name = "addplayer", ObjIDSender = player }
		
		-- setStunned(self, player)
		local objects = self:GetAllActivityUsers{}.objects
		
		for i = 1, #objects do
		
			if self:GetVar("Con.inActivity_"..i) == nil then
				self:SetVar("Con.inActivity_"..i, objects[i]:GetID() )
				break
			end
		end
			
	end

end

function ActivityRemove(self, player)

	if getObjectByName(self, "ActivityObj") then

    	local ActivityObj = getObjectByName(self, "ActivityObj")
    	ActivityObj:NotifyObject{name = "removeplayer", ObjIDSender = player }	

	end
	
end


function onNotifyObject(self,msg)


	if msg.name == "reset" then
	
		Con = {}
		for i = 1, 4 do 

			Con["inActivity_"..i] = nil

		end

		self:SetVar("Con",Con)	
	
		local gate = self:GetObjectsInGroup{ group = "gate" ,ignoreSpawners = true }.objects
		gate[1]:GoToWaypoint{iPathIndex = 0, bAllowPathingDirectionChange = true}
	end
	

	
end




