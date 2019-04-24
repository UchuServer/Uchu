require('o_mis')

con = {}

con["null_Object_to_spawn"] = 9283
con["null_Object_group_name"] = "emp"

function onStartup(self) 


	if not self:GetVar("GroupName") and not self:GetVar("nullID") then

		local config = { {"groupID", con["null_Object_group_name"] } }

			local Markpos = self:GetPosition().pos 
			local Markrot = self:GetRotation()		

			RESMGR:LoadObject { objectTemplate = con["null_Object_to_spawn"] , x = Markpos.x ,
			y = Markpos.y , z = Markpos.z ,rw = Markrot.w , rx = Markrot.x, ry = Markrot.y  , 
			rz = Markrot.z, owner = self, configData = config}  	
	else
	
		local config = { {"groupID", self:GetVar("GroupName") } }


			local Markpos = self:GetPosition().pos 
			local Markrot = self:GetRotation()		

			RESMGR:LoadObject { objectTemplate = self:GetVar("nullID")  , x = Markpos.x ,
			y = Markpos.y , z = Markpos.z ,rw = Markrot.w , rx = Markrot.x, ry = Markrot.y  , 
			rz = Markrot.z, owner = self, configData = config}  
	
	
	end
	
end



