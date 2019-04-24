--------------------------------------------------------------
-- Mission Helper Functions
--------------------------------------------------------------
MISSION_AVAILABLE = 1
MISSION_ACTIVE = 2
MISSION_READY_TO_COMPLETE = 3
MISSION_COMPLETE = 4
MISSION_FAIL = 5
MISSION_READY_TO_COMPLETE_REPORTED = 6


--------------------------------------------------------------
-- Add mission help to the data structure
--------------------------------------------------------------
function AddMissionHelp(self, dataTable, missionID, state, myFunc)

    if (dataTable == nil) then
        return false
    end
    
    if (dataTable[missionID] == nil) then
        dataTable[missionID] = {}
    end
    
    if (dataTable[missionID][state] == nil) then
        MISSION_STATE = {action = myFunc}
        dataTable[missionID][state] = MISSION_STATE
    end

end


--------------------------------------------------------------
-- Bit Checking Functions
--------------------------------------------------------------
function bit(p)
  return 2 ^ (p - 1)  -- 1-based indexing
end

-- Typical call:  if hasbit(x, bit(3)) then ...
function hasbit(x, p)
  return x % (p + p) >= p       
end


--------------------------------------------------------------
-- Performs help actions for an object based on missions and mission states
--------------------------------------------------------------
function ActivateHelp(self, dataTable, missionID, missionState)

    if (dataTable[missionID] ~= nil) then

        -- set the state we are looking for
        local checkState = 0
        if ( hasbit(missionState, bit(MISSION_AVAILABLE)) ) then
            checkState = MISSION_AVAILABLE
        elseif ( hasbit(missionState, bit(MISSION_READY_TO_COMPLETE_REPORTED)) ) then
            checkState = MISSION_READY_TO_COMPLETE_REPORTED
        elseif ( hasbit(missionState, bit(MISSION_READY_TO_COMPLETE)) ) then
            checkState = MISSION_READY_TO_COMPLETE
        elseif ( hasbit(missionState, bit(MISSION_COMPLETE)) ) then
            checkState = MISSION_COMPLETE
        elseif ( hasbit(missionState, bit(MISSION_FAIL)) ) then
            checkState = MISSION_FAIL
        elseif ( hasbit(missionState, bit(MISSION_ACTIVE)) ) then
            checkState = MISSION_ACTIVE
        end

        -- good state, check state for mission and do action
        if (checkState ~= 0 and dataTable[missionID][checkState] and dataTable[missionID][checkState].action ~= nil) then
            
            dataTable[missionID][checkState].action(self)
            return true

        end

    end
    return false
    
end