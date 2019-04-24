require('State')
require('o_StateCreate')
require('o_mis')
require('o_Main')

-- All the lot nums for the maze
LOT_NUMS = {}
LOT_NUMS[1] = { 3626, 3627, 3628, 3700 }


function onStartup(self)
	 print("TB Bench Startup")
end


function onTripleBuildSelect(self, msg)
	print("TripleBuildSelect")
	
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
	self:Die{killType = "SILENT"} 
	RESMGR:LoadObject { objectTemplate = msg.lotnum, x= mypos.x, y= mypos.y , z= mypos.z, owner = self, rw= myRot.w, rx= myRot.x, ry= myRot.y, rz = myRot.z }
end

