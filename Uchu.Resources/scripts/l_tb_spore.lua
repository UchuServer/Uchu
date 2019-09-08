require('o_mis')

-- All the lot nums for the maze
LOT_NUMS = {}
LOT_NUMS[1] = { 3712, 3713, 3714, 3712 } --3712 chicken 3713 skunk 3714 sheep


function onGetOverridePickType(self, msg)
    msg.ePickType = 14
	return msg
end


function onClientUse(self, msg)
	-- Send every LOT number that isn't us
	
	local ournum = 1;
	-- Store all the lots nums that arent us
	-- TODO This should be in a table so that it would be dynamic, but not sure how to store one with SetVar yet
	local slot = self:GetVar("ourslot")
	for i = 1, #LOT_NUMS[slot] do
		--if(self:GetVar("ourlotnum") ~= LOT_NUMS[slot][i] or ournum == 3 and i == 4) then 
			if(ournum == 1) then
				self:SetVar("lot1", LOT_NUMS[slot][i])
			elseif(ournum == 2) then
				self:SetVar("lot2", LOT_NUMS[slot][i])
			elseif(ournum == 3) then
				self:SetVar("lot3", LOT_NUMS[slot][i])
			end
			
			ournum = ournum + 1
			
			--local nextLot = #OUR_LOTNUMS + 1
			--OUR_LOTNUMS[nextLot] = LOT_NUMS[i]
		--end
	end
	
	--print("TRIPLE: " .. self:GetVar("lot1") .." " .. self:GetVar("lot2") .. " " .. self:GetVar("lot3"))
	UI:DisplayTripleBuild( self, true, { self:GetVar("lot1"), self:GetVar("lot2"), self:GetVar("lot3") } )
	
	--print("TRIPLE: " .. OUR_LOTNUMS[1] .." " .. OUR_LOTNUMS[2] .. " " .. OUR_LOTNUMS[3])
	--UI:DisplayTripleBuild( self, true, { OUR_LOTNUMS[1], OUR_LOTNUMS[2], OUR_LOTNUMS[3] } )
end


function onStartup(self)
	-- Save our LOT number
	self:SetVar("ourlotnum", self:GetLOT{}.objtemplate)
	for i = 1, #LOT_NUMS do
		for a = 1, #LOT_NUMS[i] do
			if(self:GetVar("ourlotnum") == LOT_NUMS[i][a]) then
				self:SetVar("ourslot", i)
				return
			end
		end
	end
end
