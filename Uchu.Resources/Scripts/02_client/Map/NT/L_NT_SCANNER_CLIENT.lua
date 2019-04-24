--------------------------------------------------------------

-- L_NT_SCANNER_CLIENT.lua

-- Runs animation and behaviors of Security Scanner object
-- created abeechler ... 5/4/11

--------------------------------------------------------------

local scannerLotID = 13403		-- Scanner object valid for playing the scan FX
local scannerAnimID = "scan"	-- The animation to play on triggered scan events

----------------------------------------------
-- Catch player scanner collisions and process 
-- functions when applicable
----------------------------------------------
function onCollisionPhantom(self, msg)
	-- Have we sensed a collision, and are we ready to run the scanner?
	if(self:GetVar("bIsScanning")) then return end

	-- Process the scan event for every valid scanner object in my group
	local myGroup = string.gsub(self:GetVar("groupID"),"%;","")
	local scannerObjects = self:GetObjectsInGroup{group = myGroup, ignoreSpawners = true, ignoreSelf = true}.objects
		
	for i, scanObject in ipairs(scannerObjects) do
		if(scanObject:Exists()) then
			-- Do we have a valid scanner object?
			if(scanObject:GetLOT().objtemplate == scannerLotID) then
				-- Lock the scanner animation until complete
				self:SetVar("bIsScanning", true)
				-- Run the scanner animation
				scanObject:PlayAnimation{ animationID = scannerAnimID, fPriority = 2.0}
				-- Run an unlock timer to allow subsequent scans
				local scanAnimTime = scanObject:GetAnimationTime{animationID = scannerAnimID}.time or 3
				GAMEOBJ:GetTimer():AddTimerWithCancel(scanAnimTime, "ScanComplete", self)
				-- We found a valid scanner to run, stop looping
				break
			end
		end
	end
end

----------------------------------------------
-- Process timer events to allow subsequent scans
----------------------------------------------
function onTimerDone (self,msg)
	if (msg.name == "ScanComplete") then
		-- Ready to scan again
		self:SetVar("bIsScanning", false)
	end
end
