using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication2.ServiceReference1;
using System.ServiceModel;

namespace ConsoleApplication2
{
    class Program
    {

        private static RightNowSyncPortClient _client;
        public Contact c;

        static void Main(string[] args)
        {
            Program program = new Program();

            //----- Practica operacion batch
            program.operacionesBatch(program);
            Console.ReadKey();
            Console.WriteLine("Presione una tecla cualquiera");
        }

        public Program()
        {
            _client = new RightNowSyncPortClient();
            _client.ClientCredentials.UserName.UserName = "lucas";
            _client.ClientCredentials.UserName.Password = "password1";
        }

        public void operacionesBatch(Program program)
        {
            BatchRequestItem[] brItems = new BatchRequestItem[6];
            brItems[0] = gettearIncidenteBatch(24876);
            brItems[1] = crearContactoBatch("Antho");
            brItems[2] = gettearIncidenteBatch(24877);
            brItems[2].CommitAfter = true;
            brItems[2].CommitAfterSpecified = true;
            brItems[3] = crearContactoBatch("Juanma");
            brItems[4] = gettearIncidenteBatch(24875);
            brItems[5] = destroyContactBatch(564);

            try
            {
                ClientInfoHeader cih = new ClientInfoHeader { AppID = "operacion batch" };
                BatchResponseItem[] batchRes = _client.Batch(cih, brItems);

                program.ProcessResponse(batchRes);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            pressAnyKey();

        }

        public BatchRequestItem gettearIncidenteBatch(long id)
        {
            BatchRequestItem brGet = new BatchRequestItem();
            GetMsg getmsg = new GetMsg();
            GetProcessingOptions go = new GetProcessingOptions { FetchAllNames = false };
            getmsg.ProcessingOptions = go;
            Incident incident = new Incident();
            incident.ID = new ID();
            incident.ID.id = id;
            incident.ID.idSpecified = true;
            getmsg.RNObjects = new RNObject[] { incident };
            brGet.Item = getmsg;
            return brGet;
        }

        public BatchRequestItem crearContactoBatch(string name)
        {
            BatchRequestItem brCreate = new BatchRequestItem();
            CreateMsg createmsg = new CreateMsg();
            CreateProcessingOptions co = new CreateProcessingOptions { SuppressExternalEvents = true, SuppressRules = true };
            createmsg.ProcessingOptions = co;

            Contact contact = new Contact();
            contact.Name = new PersonName();
            contact.Name.First = name;

            createmsg.RNObjects = new RNObject[] { contact };
            brCreate.Item = createmsg;
            return brCreate;

        }

        public BatchRequestItem destroyContactBatch(long id)
        {
            BatchRequestItem bri = new BatchRequestItem();
            DestroyMsg msgDes = new DestroyMsg();

            DestroyProcessingOptions dpo = new DestroyProcessingOptions { SuppressExternalEvents = false, SuppressRules = false };
            Contact contact = new Contact { ID = new ID { id = id, idSpecified = true } };

            msgDes.RNObjects = new RNObject[] { contact };
            bri.Item = msgDes;
            return bri;

        }

        public void ProcessResponse(BatchResponseItem[] batchRes)
        {
            foreach (BatchResponseItem res in batchRes)
            {
                if (res.Item is CreateResponseMsg)
                {
                    CreateResponseMsg crm = (CreateResponseMsg)res.Item;
                    RNObject[] objetosCreados = crm.RNObjectsResult;
                    foreach (RNObject o in objetosCreados)
                    {
                        Console.WriteLine("Se creo id" + o.ID.id);
                    }
                }
                if (res.Item is GetResponseMsg)
                {
                    GetResponseMsg grm = (GetResponseMsg)res.Item;
                    RNObject[] objetosGetteados = grm.RNObjectsResult;
                    foreach (RNObject o in objetosGetteados)
                    {
                        Console.WriteLine("Se getteo id" + o.ID.id);
                    }

                }
                if (res.Item is RequestErrorFaultType)
                {
                    RequestErrorFaultType fault = (RequestErrorFaultType)res.Item;
                    Console.WriteLine(fault.exceptionMessage);
                }
            }

        }

        public void pressAnyKey()
        {
            Console.WriteLine("Presione una tecla cualquiera");
            Console.ReadKey();
        }
    }
}
