task :default => :rexap

SL  = 'c:\dev\vsl1s\Merlin\Main\Utilities\Silverlight\x86ret'
CSC = 'C:\Windows\Microsoft.NET\Framework\v3.5\csc.exe'
DLR = 'c:\dev\vsl1s\Merlin\Main\Bin\Silverlight Debug'
LOCAL_DLR = File.dirname(__FILE__) + '\dlr'
CHR = "#{DLR}\\Chiron.exe"

SYSTEM_REFERENCES = %W(mscorlib System System.Core System.Net System.Windows System.Windows.Browser)
DLR_REFERENCES    = %W(Microsoft.Scripting.Silverlight Microsoft.Scripting Microsoft.Dynamic Microsoft.Scripting.Core Microsoft.Scripting.ExtensionAttribute IronRuby IronRuby.Libraries IronPython IronPython.Modules)

task :update_dlr do
  FileUtils.mkdir_p(LOCAL_DLR)
  FileUtils.cp(DLR_REFERENCES.map{|i| "#{DLR}\\#{i}.dll"}, LOCAL_DLR)
end

task :build do
  puts "Building lib/Eggs.dll"
  options = { :t => 'library', :out => 'lib/Eggs.dll', :nowarn => '1685', :debug => 'full' }
  flags = [:'nostdlib+', :noconfig, :'debug+']
  flags << :nologo unless ENV['DEBUG']
  csc 'src/*.cs', flags,
    convert_to_options(SYSTEM_REFERENCES, SL,
      convert_to_options(DLR_REFERENCES, LOCAL_DLR, options))
end

task :xap => :build do
  puts "Generating eggs.xap"
  flags = ENV['DEBUG'] ? [] : [:silent]
  eggsxap = File.dirname(__FILE__) + '/eggsxap'
  FileUtils.mkdir_p eggsxap
  FileUtils.cp DLR_REFERENCES.map{|i| "#{LOCAL_DLR}\\#{i}.dll"}, eggsxap
  FileUtils.cp_r "#{File.dirname(__FILE__) + '/lib'}/.", eggsxap
  chr flags, :directory => eggsxap, :xapfile => 'eggs.xap'
end

task :clean do
  puts "Cleaning"
  require 'fileutils'
  FileUtils.rm 'lib/Eggs.dll' if File.exist?('lib/Eggs.dll')
  FileUtils.rm 'lib/Eggs.pdb' if File.exist?('lib/Eggs.pdb')
  FileUtils.rm 'eggs.xap' if File.exist?('eggs.xap')
  FileUtils.rm_r 'eggsxap' if File.exist?('eggsxap')
end

task :rebuild => [:clean, :update_dlr, :build]
task :rexap   => [:clean, :update_dlr, :build, :xap]

def convert_to_options(assemblies, path, base_opts = {})
  assemblies.inject(base_opts) do |opts, ref|
    opts[:r] ||= []
    opts[:r] << "\"#{path.gsub('/', '\\')}\\#{ref}.dll\""
    opts
  end
end

def csc(files, flags, options)
  exe(CSC, files.gsub('/', '\\'), flags, options)
end

def chr(flags, options)
  exe(CHR, nil, flags, options)
end

def exe(cmd, args, flags, options)
  str_options = flags.inject("") {|flgs, flg| flgs << " /#{flg}"; flgs}
  str_options = options.inject(str_options) do |opts, (key,value)|
    [value].flatten.each do |v|
      opts << " /#{key}:#{v}"; opts
    end
    opts
  end
  cmd = "#{cmd}#{str_options} #{args}"
  puts cmd if ENV['DEBUG']
  system cmd
end
