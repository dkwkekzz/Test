using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpeakingLanguage.Core
{
    static class Loop
    {
        public static void Notify()
        {
            Task.Factory.StartNew(() =>
            {
                var handle = Locator.Get<SyncHandle>();
                var inputBuffer = Locator.Get<SourceBuffer>();
                //var outputBuffer = Locator.Get<SourceBuffer<RenderingSource>>();
                var jobIter = Locator.Get<JobPartitioner>();
                var ticker = Locator.Get<Ticker>();
                var logger = Locator.Get<Logger>();

                logger.Log($"create Notifier");

                handle.WaitForComplete();

                var isRun = true;
                while (isRun)
                {
                    ticker.Enter();
                    try
                    {
                        var count = inputBuffer.Count;
                        if (0 < count)
                        {
                            using (jobIter.Take(Config.COUNT_DEFAULT_WORKER, count))
                            {
                                // outbuffer은 어디에선가 사용했어야 한다...
                                // 새로운 값을 넣기 전에 빼야 한다. 
                                //outputBuffer.Flush();

                                logger.Log($"signal Notifier");
                                handle.SignalWorking();
                                logger.Log($"waiting Notifier");
                                handle.WaitForComplete();

                                inputBuffer.Flush();
                                //outputBuffer.Commit();
                            }
                        }
                    }
                    catch (NotImplementedException e) { logger.LogError($"notifier: {e.Message}/{e.StackTrace}"); }
                    catch (ArgumentException e) { logger.LogError($"notifier: {e.Message}/{e.StackTrace}"); }
                    catch (Exception e) { logger.LogError($"notifier: {e.Message}/{e.StackTrace}"); }
                    finally
                    {
                        int leg = ticker.Leave();
                        if (leg < 0)
                        {
                            Thread.Sleep(-leg);
                        }

                        logger.Log(ticker.Log);
                    }
                }
            });
        }

        public static void Update(int id)
        {
            Task.Factory.StartNew(() =>
            {
                var handle = Locator.Get<SyncHandle>();
                var jobIter = Locator.Get<JobPartitioner>();
                var sysList = Locator.Get<SystemCollection>();
                var logger = Locator.Get<Logger>();

                logger.Log($"create Updater");

                handle.SignalCompleted(id);

                var isRun = true;
                while (isRun)
                {
                    logger.Log($"wait: {id}");
                    handle.WaitForWork(id);

                    try
                    {
                        while (jobIter.MoveNext())
                        {
                            var jobCk = jobIter.Current;
                            for (int srcIndex = jobCk.begin; srcIndex != jobCk.end; srcIndex++)
                            {
                                int sysIndex = 0;
                                while (sysIndex < sysList.Count)
                                {
                                    var sys = sysList[sysIndex];
                                    var res = sys.OnUpdateAsParallel(srcIndex);
                                    switch (res.res)
                                    {
                                        case FrameResult.Result.Success:
                                            sysIndex += res.skip;
                                            break;
                                        case FrameResult.Result.Fail:
                                            sysIndex = sysList.Count;
                                            break;
                                        default:
                                            throw new NotImplementedException($"wrong parallel visit in {sys.GetType().Name}");
                                    }
                                }
                            }
                        }

                        Thread.Yield();
                    }
                    catch (NotImplementedException e) { logger.LogError($"updater: {e.Message}/{e.StackTrace}"); }
                    catch (KeyNotFoundException e) { logger.LogError($"updater: {e.Message}/{e.StackTrace}"); }
                    catch (ArgumentException e) { logger.LogError($"updater: {e.Message}/{e.StackTrace}"); }
                    catch (Exception e) { logger.LogError($"updater: {e.Message}/{e.StackTrace}"); }
                    finally
                    {
                        handle.SignalCompleted(id);
                        logger.Log($"complete: {id}");
                    }
                }
            });
        }
    }
}
