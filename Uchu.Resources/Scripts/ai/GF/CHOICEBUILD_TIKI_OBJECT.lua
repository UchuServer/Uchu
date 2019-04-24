-- copy of CHOICEBUILD_STAGE_OBJECT.lua --- on the choicebuild object
require('o_mis')

--need to reset the TikiSet to false on all the quickbuilds either on ramp build or on distory
-- or manually have the tiki heads reset in the database

 
function onChoicebuildComplete(self, msg) 	

	local manager = self:GetObjectsInGroup{ group = 'TikiPuzzleManager', ignoreSpawners = true }.objects[1]
	
    local groupID = split(self:GetVar('groupID'), ';')[2]
    -- check to see if the object group matches the choicebuild index
    if groupID == 'LargeTiki' and msg.index == 2 then
        print("large tiki in the right spot")
        self:SetVar('TikiSet', true)
        
    elseif groupID == "MediumTiki" and msg.index == 1 then  
        print("meduim tiki in the right spot")
        self:SetVar('TikiSet', true)
        --manager:NotifyObject{ ObjIDSender = self, name = "CorrectTimer" }
    elseif groupID ==  "SmallTiki" and msg.index == 0 then  
        print("small tiki in the right spot")
        self:SetVar('TikiSet', true)
        --manager:NotifyObject{ ObjIDSender = self, name = "CorrectTimer" }
    --else
		--GAMEOBJ:GetTimer():AddTimerWithCancel(30, "WrongTikiTimer", self )
		--print ("wrong tiki timer start !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!")
	end			
	manager:NotifyObject{ ObjIDSender = self, name = "CorrectTimer" }
	local group = self:GetObjectsInGroup{ group = 'CBGroup', ignoreSpawners = true }.objects		
	local tikiTest = 0
	for i = 1, #group do   
		-- test each object in table group to see if GetVar('TikiSet') is true, if so add 1 to tikiTest
		if group[i]:GetVar('TikiSet') then		
			tikiTest = tikiTest + 1
			
		end
	end
	
	
	
	-- if all objects have been created in the correct locations then we are going to spawn the ramp
	if tikiTest == 3 then
		print('spawn in ramp')
		--LEVEL:ActivateSpawner("Ramp")
		manager:NotifyObject{ ObjIDSender = self, name = "RampTimer" }
		
		local rampSpawner = LEVEL:GetSpawnerByName("Ramp")
		if rampSpawner then
			rampSpawner:SpawnerActivate()
		end
		
	end
	
end

function onTimerDone(self, msg)  
	if msg.name == "WrongTikiTimer" then
		print ("wrong timer done")
		self:Die{ killType = "SILENT" }
	end
end
