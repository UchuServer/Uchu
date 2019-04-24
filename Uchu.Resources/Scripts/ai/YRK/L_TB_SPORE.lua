require('State')
require('o_StateCreate')
require('o_mis')
require('o_Main')




--------- HORRIBLY UGLY TEST SCRIP. JUST A TRIPLE BUILD EXCERCISE AND NOT FOR PRODUCTION LEVEL

-- All the lot nums for the maze
LOT_NUMS = {}
LOT_NUMS[1] = { 3712, 3713, 3714, 3712 } --3712 chicken 3713 skunk 3714 sheep


function onStartup(self)

	-- 3 second timer on startup before we set all our params
	GAMEOBJ:GetTimer():AddTimerWithCancel(3, "startTimer", self)
	self:SetVar("HasStarted", 0 )
	self:SetVar("dying", 0)
	storeHomePoint(self)
	
end


function DoWander(self)
	--print("DoWander")
	local  mypos = self:GetPosition().pos 
	local  rpos = getRandomPos(self, mypos, 15)
	
	GAMEOBJ:GetTimer():CancelTimer("wanderTimer", self)
	GAMEOBJ:GetTimer():AddTimerWithCancel(12, "wanderTimer", self)
	
	--print("my: " .. mypos.x.. " ".. " " .. mypos.y .. " ".. mypos.z)
	--print("r:  " .. rpos.x.. " ".. " " ..rpos.y .. " ".. rpos.z)
	
	-- Every once in a awhile we go back to the tether point
	local r = math.random(1, 8)
	if(r == 4) then
		--print("going to tether: " .. self:GetVar("ourlotnum"))
		self:GoTo { speed = 0.50, target = { x = tetherpos.x, z = tetherpos.z, y = tetherpos.y} }
	else
		self:GoTo { speed = 0.50, target = { x = rpos.x, z = rpos.z, y = rpos.y} }
	end
end


function onTripleBuildSelect(self, msg)
	local canswap = 0
	
	-- Make sure we are allowed to spawn the piece they are requesting
	for i = 1, #LOT_NUMS do
		for a = 1, #LOT_NUMS[i] do
			if(LOT_NUMS[i][a] == msg.lotnum) then
				canswap = 1 -- how do you break out of a loop in lua?
			end
		end
	end
	
	if(canswap == 0) then
		print("CANT SWAP")
		return
	end
		
	local mypos = self:GetPosition().pos
	local myRot = self:GetRotation()
	
	self:SetVar("dying", 1)
	self:Die{killType = "SILENT"} 
	RESMGR:LoadObject { objectTemplate = msg.lotnum, x= mypos.x, y= mypos.y , z= mypos.z, owner = self, rw= myRot.w, rx= myRot.x, ry= myRot.y, rz = myRot.z }
end


function onProximityUpdate(self, msg)

	-- If we havent started then dont do anything
	if(self:GetVar("HasStarted") == 0) then
		return
	end
		
	-- If we are dying then don't do anything
	if(self:GetVar("dying") == 1) then
		return
	end

	if msg.status == "ENTER" then
	
		-- get the target
        local newobj = msg.objId
        local newlot = newobj:GetLOT{}.objtemplate
        
        -- if it is not one of whatever we are ....
        if(newlot == self:GetVar("ourlotnum")) then
			return
        end
        
        -- CHICKEN
        -- If we are the chicken then chase skunks and run from sheep
        if(self:GetVar("ourlotnum") == 3712) then
        
			-- It this is our attack radius
			if(msg.name == "attack") then
				if(newlot == 3713) then -- skunk
					--print("KILLING OPPONENT SKUNK")
					newobj:NotifyObject{ name="swapobjects", param1 = self:GetVar("ourlotnum") }
					self:SetVar("NoWander", 0)
					DoWander(self)
				end
			
			else
				if(newlot == 3713) then -- skunk
					-- chase
					self:FollowTarget
					{ 
						targetID = newobj, 
						radius = 0,
						speed = 3,	
						keepFollowing = true 
					}
					
					self:SetVar("NoWander", 1)
					--print("no wander is 1 chicken following skunk")
				elseif(newlot == 3714) then -- sheep
					-- evade
					--local mySpeed = self:GetSpeed().speed
					self:EvadeTarget
					{ 
						targetID = newobj, 
						radius = 20,
						speed = 1,
						keepEvading = true 
					}
					self:SetVar("NoWander", 1)
					--print("no wander is 1 chicken evading sheep")
				end
			end
        end
        
        -- SHEEP
        -- If we are the sheep then chase the chicken and evade the skunk
        if(self:GetVar("ourlotnum") == 3714) then
			if(msg.name == "attack") then
				if(newlot == 3712) then -- chicken
					--print("KILLING OPPONENT CHICKEN")	
					 newobj:NotifyObject{ name="swapobjects", param1 = self:GetVar("ourlotnum") }
					 self:SetVar("NoWander", 0)
					 DoWander(self)
				end
			
			else
				if(newlot == 3712) then -- chicken
					--print("FOLLOWING CHICKEN")
					-- chase
					self:FollowTarget
					{ 
						targetID = newobj, 
						radius = 0,
						speed = 3,
						keepFollowing = true 
					}
					self:SetVar("NoWander", 1)
					--print("no wander is 1 sheep following chicken")
				elseif(newlot == 3713) then -- skunk
					-- evade
					--local mySpeed = self:GetSpeed().speed
					self:EvadeTarget
					{	 
						targetID = newobj, 
						radius = 20,
						speed = 1,	
						keepEvading = true 
					}
					self:SetVar("NoWander", 1)
					--print("no wander is 1 sheep evading skunk")
				end
			end
        end
        
        -- SKUNK
        -- If we are the skunk then chase the sheep and evade the chicken
        if(self:GetVar("ourlotnum") == 3713) then
			if(msg.name == "attack") then
				if(newlot == 3714) then -- sheep
					--print("KILLING OPPONENT SHEEP")	
					newobj:NotifyObject{ name="swapobjects", param1 = self:GetVar("ourlotnum") }
					self:SetVar("NoWander", 0)
					DoWander(self)
				end
			
			else
				if(newlot == 3714) then -- sheep
					--print("FOLLOWING SHEEP")
					-- chase
					self:FollowTarget
					{ 
						targetID = newobj, 
						radius = 0,
						speed = 3,
						keepFollowing = true 
					}
					self:SetVar("NoWander", 1)
					--print("no wander is 1 skunk evading following sheep")
				elseif(newlot == 3712) then -- chicken
					-- evade
					local mySpeed = self:GetSpeed().speed
					self:EvadeTarget
					{	 
						targetID = newobj, 
						radius = 20,
						speed = 1,	
						keepEvading = true 
					}
					self:SetVar("NoWander", 1)
					--print("no wander is 1 skunk evading chicken")
				end
			end
        end
    
	end
end


function onNotifyObject(self, msg)
    
    if(self:GetVar("dying") == 1) then
		return
    end
    
    -- TODO verify the object they want us to be is one we can be
    if (msg.name == "swapobjects") then
		local mypos = self:GetPosition().pos
		local myRot = self:GetRotation()
		self:SetVar("dying", 1)
		self:Die{killType = "SILENT"} 
		
		RESMGR:LoadObject { objectTemplate = msg.param1, x= mypos.x, y= mypos.y , z= mypos.z, owner = self, rw= myRot.w, rx= myRot.x, ry= myRot.y, rz = myRot.z }
    end
end


function onArrived(self, msg)
    --print("On arrived")
    if(self:GetVar("NoWander") == 0) then
		DoWander(self)
    end
end


--onLeftTetherRadius = function(self, msg)
--	print("LEFT TETHER: " .. self:GetVar("ourlotnum"))
--	if(self:GetVar("NoWander") == 0) then
--		print("what?")
--		print("x y z: " .. self:GetVar("myx") .. " " .. self:GetVar("myy") " " .. self:GetVar("myz"))
--		self:GoTo { speed = 4, target = { x = self:GetVar("myx"), z = self:GetVar("myz"), y = self:GetVar("myy")} }
--	end
--end


onTimerDone = function(self, msg)

	if msg.name == "startTimer" then
		-- print("SPORE Startup")
		-- make this a constant somewhere
		
		-- Evade Follow radius
		self:SetProximityRadius { radius = 20 }
		self:SetProximityRadius { radius = 2, name = "attack" };
     
		 -- Save our LOT number
		self:SetVar("ourlotnum", self:GetLOT{}.objtemplate)
		
		-- Misc variables
		self:SetVar("NoWander", 0 )
		self:SetVar("HasStarted", 1 )
		
		-- Global tether position
		tetherpos = GAMEOBJ:GetWaypointPos("SporeTether", 2)
		--self:SetTetherPoint { tetherPt = pos,  radius = 30 }
	
		-- Start wandering
		DoWander(self)
	end
	
	if msg.name == "wanderTimer" then
		--print("WANDER TIMER")
		if(self:GetVar("NoWander") == 0) then
			--print("WANDER TIMER IN")
			DoWander(self)
		end
	end
	
end


function onStoppedEvading(self, msg)
	--print("Stopped evading")
	self:SetVar("NoWander", 0 )
	DoWander(self)
end