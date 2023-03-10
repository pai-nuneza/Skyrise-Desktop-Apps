using System;
using System.Timers;

namespace TimeKeepingManager.Service
{
    //Author : Nilo L. Luansing Jr. <nlluansing@n-pax.com>

    class ServiceScheduler
    {
        // initialize scheduler variables
        System.Timers.Timer TimeKeepingServiceTimer = new System.Timers.Timer();
        public void InitializeServiceSchedule()
        {
            try
            {
                //for testing set to evry seconds
                TimeKeepingServiceTimer.Interval = 45000; ;
                TimeKeepingServiceTimer.Start();
                GC.KeepAlive(TimeKeepingServiceTimer);
                TimeKeepingServiceTimer.Elapsed += new System.Timers.ElapsedEventHandler(TimeKeepingServiceScheduleChecker);
            }
            catch (Exception ex)
            {
            }
        }

        protected void TimeKeepingServiceScheduleChecker(object obj, ElapsedEventArgs e)
        {
            //call methods for starting the service
            ServiceSchedulerProcessCaller ProcessCaller = new ServiceSchedulerProcessCaller();
            ProcessCaller.CallTimeKeepingManagerService();
        }

        // class calling process
        class ServiceSchedulerProcessCaller
        {
            ServiceScheduler service_var = new ServiceScheduler();
            // check if current time is equal to scheduled service : call service process
            public void CallTimeKeepingManagerService()
            {
                // calling method from swiping ftp
                TimeKeepingManagerService _uploading = new TimeKeepingManagerService();
                _uploading.StartServiceConsoleProcess();
            }
        }
    }
}
